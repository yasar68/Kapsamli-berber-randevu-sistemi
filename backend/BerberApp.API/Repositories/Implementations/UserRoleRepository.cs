using BerberApp.API.Data;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BerberApp.API.Repositories.Implementations
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserRole>> GetAllAsync()
        {
            return await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role) // Burada Role olmalı RoleName değil
                .ToListAsync();
        }

        public async Task<UserRole?> GetByIdAsync(int id)
        {
            return await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)  // Burada da Role olmalı
                .FirstOrDefaultAsync(ur => ur.Id == id);
        }

        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.User)
                .ToListAsync();
        }

        public async Task AddAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserRole userRole)
        {
            _context.UserRoles.Update(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserRole userRole)
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        }
        public async Task<List<string>> GetRolesByUserIdAsync(int userId)
        {
            // Varsayalım UserRole tablosunda UserId ve RoleId var
            // Role tablosunda RoleName var

            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role) // Role navigasyon property varsa
                .Select(ur => ur.Role != null ? ur.Role.Name : string.Empty)
                .ToListAsync();
        }
    }
}
