using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.DTOs.Service;

namespace BerberApp.API.Services.Interfaces
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceViewDto>> GetAllAsync();
        Task<ServiceViewDto?> GetByIdAsync(int id);
        Task<ServiceViewDto> CreateAsync(CreateServiceDto createServiceDto);
        Task<ServiceViewDto?> UpdateAsync(UpdateServiceDto updateServiceDto);
        Task<bool> DeleteAsync(int id);
    }
}
