using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.Models;

namespace BerberApp.API.Repositories.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<IEnumerable<UserRole>> GetAllAsync();
        Task<UserRole?> GetByIdAsync(int id);
        Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId);
        Task AddAsync(UserRole userRole);
        Task UpdateAsync(UserRole userRole);
        Task DeleteAsync(UserRole userRole);
        Task<List<string>> GetRolesByUserIdAsync(int userId);

    }
}
