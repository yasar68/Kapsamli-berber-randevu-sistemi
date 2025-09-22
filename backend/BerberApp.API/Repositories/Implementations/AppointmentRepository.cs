using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BerberApp.API.Data;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BerberApp.API.Repositories.Implementations
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AppointmentRepository> _logger;

        public AppointmentRepository(ApplicationDbContext context, ILogger<AppointmentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(appointmentService => appointmentService.Service)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(appointmentService => appointmentService.Service)
                .ToListAsync();
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Entry(appointment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Appointment {Id} updated successfully", appointment.Id);
        }

        public async Task DeleteAsync(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Appointments.AnyAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetByUserIdAsync(int userId)
        {
            return await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(appointmentService => appointmentService.Service)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByBarberIdAsync(int barberId)
        {
            return await _context.Appointments
                .Where(a => a.BarberId == barberId)
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(appointmentService => appointmentService.Service)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsBetweenDatesAsync(
            DateTime start,
            DateTime end,
            int? barberId = null)
        {
            var query = _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(appointmentService => appointmentService.Service)
                .Where(a => a.AppointmentDate.Date >= start.Date &&
                           a.AppointmentDate.Date <= end.Date);

            if (barberId.HasValue && barberId > 0)
            {
                query = query.Where(a => a.BarberId == barberId);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status)
        {
            return await _context.Appointments
                .Where(a => a.Status == status)
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(appointmentService => appointmentService.Service)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByStatusAndDateRangeAsync(
            AppointmentStatus status,
            DateTime startDate,
            DateTime endDate,
            int? barberId = null)
        {
            var query = _context.Appointments
                .Where(a => a.Status == status &&
                           a.AppointmentDate.Date >= startDate.Date &&
                           a.AppointmentDate.Date <= endDate.Date);

            if (barberId.HasValue && barberId > 0)
            {
                query = query.Where(a => a.BarberId == barberId);
            }

            return await query
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(appointmentService => appointmentService.Service)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByMultipleStatusesAsync(
            IEnumerable<AppointmentStatus> statuses,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? barberId = null)
        {
            var query = _context.Appointments.AsQueryable();

            if (statuses != null && statuses.Any())
            {
                query = query.Where(a => statuses.Contains(a.Status));
            }

            if (startDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date <= endDate.Value.Date);
            }

            if (barberId.HasValue && barberId > 0)
            {
                query = query.Where(a => a.BarberId == barberId);
            }

            return await query
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(appointmentService => appointmentService.Service)
                .ToListAsync();
        }

        public async Task<int> GetAppointmentCountByBarberAsync(int barberId)
        {
            return await _context.Appointments
                .Where(a => a.BarberId == barberId)
                .CountAsync();
        }

        public async Task<decimal> GetTotalRevenueByBarberAsync(int barberId)
        {
            return await _context.Appointments
                .Where(a => a.BarberId == barberId)
                .SumAsync(a => a.Price);
        }

        public async Task<decimal> GetRevenueByStatusAsync(int barberId, AppointmentStatus status)
        {
            return await _context.Appointments
                .Where(a => a.BarberId == barberId && a.Status == status)
                .SumAsync(a => a.Price);
        }

        public async Task<DateTime?> GetLastAppointmentDateAsync(int userId)
        {
            return await _context.Appointments
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => (DateTime?)a.AppointmentDate)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteAllForUserAsync(int userId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.UserId == userId)
                .ToListAsync();

            _context.Appointments.RemoveRange(appointments);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsAsync(int? customerId, int? barberId, AppointmentStatus? status)
        {
            var query = _context.Appointments.AsQueryable();

            if (customerId.HasValue)
                query = query.Where(a => a.UserId == customerId.Value);

            if (barberId.HasValue)
                query = query.Where(a => a.BarberId == barberId.Value);

            if (status.HasValue)
                query = query.Where(a => a.Status == status.Value);

            return await query
                .Include(a => a.User) // <-- Müşteri bilgisi için
                .Include(a => a.Barber) // <-- Berber bilgisi için
                .Include(a => a.AppointmentServices)
                    .ThenInclude(appointmentService => appointmentService.Service)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByBarberAndDateAsync(int barberId, DateTime date)
        {
            var dateStart = date.Date;
            var dateEnd = dateStart.AddDays(1);

            return await _context.Appointments
                .Where(a => a.BarberId == barberId
                         && a.AppointmentDate >= dateStart
                         && a.AppointmentDate < dateEnd)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(appointmentService => appointmentService.Service)
                .ToListAsync();
        }
        public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int? barberId = null)
        {
            var query = _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(asvc => asvc.Service)
                .Where(a => a.AppointmentDate.Date >= startDate.Date && a.AppointmentDate.Date <= endDate.Date);

            if (barberId.HasValue && barberId > 0)
            {
                query = query.Where(a => a.BarberId == barberId.Value);
            }

            return await query.ToListAsync();
        }
    }
}
