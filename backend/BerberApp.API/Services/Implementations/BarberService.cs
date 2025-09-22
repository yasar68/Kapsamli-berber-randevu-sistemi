using AutoMapper;
using BerberApp.API.DTOs.Barber;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using BerberApp.API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BerberApp.API.Services.Implementations
{
    public class BarberService : IBarberService
    {
        private readonly IBarberRepository _barberRepository;
        private readonly IMapper _mapper;

        public BarberService(IBarberRepository barberRepository, IMapper mapper)
        {
            _barberRepository = barberRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BarberViewDto>> GetAllBarbersAsync()
        {
            var barbers = await _barberRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BarberViewDto>>(barbers);
        }

        public async Task<BarberViewDto> GetBarberByIdAsync(int id)
        {
            var barber = await _barberRepository.GetByIdAsync(id);
            return _mapper.Map<BarberViewDto>(barber);
        }

        public async Task<BarberViewDto> CreateBarberAsync(CreateBarberDto dto)
        {
            var barber = _mapper.Map<Barber>(dto);
            barber.CreatedAt = DateTime.UtcNow;

            await _barberRepository.AddAsync(barber);
            return _mapper.Map<BarberViewDto>(barber);
        }

        public async Task<bool> UpdateBarberAsync(UpdateBarberDto dto)
        {
            // Mevcut berberi veritabanından al
            var existingBarber = await _barberRepository.GetByIdAsync(dto.Id);

            if (existingBarber == null)
                return false;

            // Değişiklikleri uygula
            existingBarber.FullName = dto.FullName;
            existingBarber.Email = dto.Email;
            existingBarber.Phone = dto.Phone;
            existingBarber.Specialties = dto.Specialties;
            existingBarber.UpdatedAt = DateTime.UtcNow;

            // Güncelleme işlemi
            await _barberRepository.UpdateAsync(existingBarber);
            return true;
        }

        public async Task<bool> DeleteBarberAsync(int id)
        {
            var barber = await _barberRepository.GetByIdAsync(id);
            if (barber == null) return false;

            await _barberRepository.DeleteAsync(barber);
            return true;
        }
    }
}