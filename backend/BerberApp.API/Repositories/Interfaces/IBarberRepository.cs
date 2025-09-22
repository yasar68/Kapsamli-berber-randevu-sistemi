using BerberApp.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BerberApp.API.Repositories.Interfaces
{
    public interface IBarberRepository
    {
        Task<bool> ExistsAsync(int id);
        Task<Barber?> GetByIdAsync(int id);
        Task<IEnumerable<Barber>> GetAllAsync();
        Task AddAsync(Barber barber);
        Task UpdateAsync(Barber barber);
        Task DeleteAsync(Barber barber);
    }
}