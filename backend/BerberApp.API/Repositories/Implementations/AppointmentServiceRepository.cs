using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using BerberApp.API.Data;

namespace BerberApp.API.Repositories.Implementations
{
    public class AppointmentServiceRepository : IAppointmentServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AppointmentService appointmentService)
        {
            _context.AppointmentServices.Add(appointmentService);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppointmentService>> GetByAppointmentIdAsync(int appointmentId)
        {
            return await _context.AppointmentServices
                .Where(x => x.AppointmentId == appointmentId)
                .Include(x => x.Service)
                .ToListAsync();
        }

        public async Task DeleteAsync(AppointmentService appointmentService)
        {
            _context.AppointmentServices.Remove(appointmentService);
            await _context.SaveChangesAsync();
        }
    }

}