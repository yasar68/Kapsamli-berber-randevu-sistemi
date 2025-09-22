using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace BerberApp.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsAsync(int id);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task HardDeleteAsync(int userId);
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task<string[]> GetRolesAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);

    }
}