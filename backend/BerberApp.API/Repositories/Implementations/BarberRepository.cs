using BerberApp.API.Data;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BerberApp.API.Repositories.Implementations
{
    public class BarberRepository : IBarberRepository
    {
        private readonly ApplicationDbContext _context;

        public BarberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            Console.WriteLine($"Checking barber with id: {id}");
            var exists = await _context.Barbers.AnyAsync(b => b.Id == id);
            Console.WriteLine($"Barber exists: {exists}");
            return exists;
        }

        public async Task<Barber?> GetByIdAsync(int id)
        {
            return await _context.Barbers
                .Include(b => b.Appointments)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Barber>> GetAllAsync()
        {
            return await _context.Barbers
                .Include(b => b.Appointments)
                .OrderBy(b => b.FullName)
                .ToListAsync();
        }

        public async Task AddAsync(Barber barber)
        {
            await _context.Barbers.AddAsync(barber);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Barber barber)
        {
            barber.UpdatedAt = DateTime.UtcNow;
            _context.Barbers.Update(barber);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Barber barber)
        {
            _context.Barbers.Remove(barber);
            await _context.SaveChangesAsync();
        }
    }
}