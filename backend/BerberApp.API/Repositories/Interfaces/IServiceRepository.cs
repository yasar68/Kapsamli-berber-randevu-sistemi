using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.Models;

namespace BerberApp.API.Repositories.Interfaces
{
    public interface IServiceRepository
    {
        Task<bool> ExistsAsync(int id);
        Task<Service?> GetByIdAsync(int id);
        Task<IEnumerable<Service>> GetAllAsync();
        Task AddAsync(Service service);
        Task UpdateAsync(Service service);
        Task DeleteAsync(Service service);
        Task<IEnumerable<Service>> GetByIdsAsync(IEnumerable<int> ids);
        
    }
}