using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.Models;

namespace BerberApp.API.Repositories.Interfaces
{
    public interface IWorkingHourRepository
    {
        Task<WorkingHour?> GetByIdAsync(int id);

        Task<IEnumerable<WorkingHour>> GetByBarberIdAsync(int barberId);

        Task AddAsync(WorkingHour workingHour);

        Task UpdateAsync(WorkingHour workingHour);

        Task DeleteAsync(WorkingHour workingHour);

        Task<bool> ExistsAsync(int barberId, DayOfWeek dayOfWeek);
        
        Task<IEnumerable<Barber>> GetAllBarbersAsync();
    }
}
