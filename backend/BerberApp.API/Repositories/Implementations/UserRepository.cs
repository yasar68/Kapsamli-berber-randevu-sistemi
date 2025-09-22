using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.Data;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BerberApp.API.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        private readonly IUserRoleRepository _userRoleRepository;

        public UserRepository(ApplicationDbContext context, IUserRoleRepository userRoleRepository)
        {
            _context = context;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            Console.WriteLine($"Checking user with id: {id}");
            var exists = await _context.Users.AnyAsync(u => u.Id == id);
            Console.WriteLine($"User exists: {exists}");
            return exists;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            // Hard delete yerine:
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(user);
        }
        public async Task HardDeleteAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<string[]> GetRolesAsync(int userId)
        {
            var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
            return userRoles.Select(ur => ur.Role?.Name ?? string.Empty).Where(name => !string.IsNullOrEmpty(name)).ToArray();
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}