using BerberApp.API.Data;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BerberApp.API.Repositories.Implementations
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            Console.WriteLine($"Checking service with id: {id}");
            var exists = await _context.Services.AnyAsync(s => s.Id == id);
            Console.WriteLine($"Service exists: {exists}");
            return exists;
        }
        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _context.Services.FindAsync(id);
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _context.Services.ToListAsync();
        }

        public async Task AddAsync(Service service)
        {
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Service service)
        {
            service.UpdatedAt = DateTime.UtcNow;
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Service service)
        {
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Service>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Services.Where(s => ids.Contains(s.Id)).ToListAsync();
        }
    }
}