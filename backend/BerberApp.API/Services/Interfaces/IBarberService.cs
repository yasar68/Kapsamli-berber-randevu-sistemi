using BerberApp.API.DTOs.Barber;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BerberApp.API.Services.Interfaces
{
    public interface IBarberService
    {
        Task<IEnumerable<BarberViewDto>> GetAllBarbersAsync();
        Task<BarberViewDto> GetBarberByIdAsync(int id);
        Task<BarberViewDto> CreateBarberAsync(CreateBarberDto dto);
        Task<bool> UpdateBarberAsync(UpdateBarberDto dto);
        Task<bool> DeleteBarberAsync(int id);
    }
}