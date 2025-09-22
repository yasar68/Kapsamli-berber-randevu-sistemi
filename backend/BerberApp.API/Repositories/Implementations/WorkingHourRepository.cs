using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BerberApp.API.Data;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BerberApp.API.Repositories.Implementations
{
    public class WorkingHourRepository : IWorkingHourRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkingHourRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WorkingHour?> GetByIdAsync(int id)
        {
            return await _context.WorkingHours.FindAsync(id);
        }

        public async Task<IEnumerable<WorkingHour>> GetAllAsync()
        {
            return await _context.WorkingHours.ToListAsync();
        }

        public async Task<IEnumerable<WorkingHour>> GetByBarberIdAsync(int barberId)
        {
            return await _context.WorkingHours
                .AsNoTracking()
                .Where(wh => wh.BarberId == barberId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int barberId, DayOfWeek dayOfWeek)
        {
            return await _context.WorkingHours
                .AnyAsync(wh => wh.BarberId == barberId && wh.DayOfWeek == dayOfWeek);
        }

        public async Task AddAsync(WorkingHour workingHour)
        {
            await _context.WorkingHours.AddAsync(workingHour);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkingHour workingHour)
        {
            _context.WorkingHours.Update(workingHour);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(WorkingHour workingHour)
        {
            _context.WorkingHours.Remove(workingHour);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Barber>> GetAllBarbersAsync()
        {
            return await _context.Barbers.ToListAsync();
        }
    }
}
