using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BerberApp.API.DTOs.Service;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using BerberApp.API.Services.Interfaces;

namespace BerberApp.API.Services.Implementations
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<IEnumerable<ServiceViewDto>> GetAllAsync()
        {
            var services = await _serviceRepository.GetAllAsync();

            return services.Select(s => new ServiceViewDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                DurationMinutes = s.DurationMinutes,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt
            });
        }

        public async Task<ServiceViewDto?> GetByIdAsync(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null) return null;

            return new ServiceViewDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                DurationMinutes = service.DurationMinutes,
                IsActive = service.IsActive,
                CreatedAt = service.CreatedAt
            };
        }

        public async Task<ServiceViewDto> CreateAsync(CreateServiceDto dto)
        {
            var newService = new Service
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DurationMinutes = dto.DurationMinutes,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _serviceRepository.AddAsync(newService);

            return new ServiceViewDto
            {
                Id = newService.Id,
                Name = newService.Name,
                Description = newService.Description,
                Price = newService.Price,
                DurationMinutes = newService.DurationMinutes,
                IsActive = newService.IsActive,
                CreatedAt = newService.CreatedAt
            };
        }

        public async Task<ServiceViewDto?> UpdateAsync(UpdateServiceDto dto)
        {
            var existingService = await _serviceRepository.GetByIdAsync(dto.Id);
            if (existingService == null) return null;

            existingService.Name = dto.Name;
            existingService.Description = dto.Description;
            existingService.Price = dto.Price;
            existingService.DurationMinutes = dto.DurationMinutes;
            existingService.IsActive = dto.IsActive;
            existingService.UpdatedAt = DateTime.UtcNow;

            await _serviceRepository.UpdateAsync(existingService);

            return new ServiceViewDto
            {
                Id = existingService.Id,
                Name = existingService.Name,
                Description = existingService.Description,
                Price = existingService.Price,
                DurationMinutes = existingService.DurationMinutes,
                IsActive = existingService.IsActive,
                CreatedAt = existingService.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null) return false;

            await _serviceRepository.DeleteAsync(service);
            return true;
        }
    }
}
