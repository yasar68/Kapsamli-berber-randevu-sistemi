using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.Models;
using BerberApp.API.DTOs.WorkingHour;

namespace BerberApp.API.Services.Interfaces
{
    public interface IWorkingHourService
    {
        Task<IEnumerable<WorkingHour>> GetByBarberIdAsync(int barberId);
        Task AddAsync(WorkingHour workingHour);
        Task UpdateAsync(WorkingHour workingHour);
        Task DeleteAsync(WorkingHour workingHour);
        Task AddDefaultWorkingHoursForTomorrowIfMissing();

        // DTO destekli metotlar
        Task AddDtoAsync(WorkingHourDto dto);
        Task UpdateDtoAsync(WorkingHourDto dto);
        Task DeleteByIdAsync(int id);
    }
}
