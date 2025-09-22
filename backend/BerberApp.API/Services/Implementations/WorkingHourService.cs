// WorkingHourService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.DTOs.WorkingHour;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using BerberApp.API.Services.Interfaces;

namespace BerberApp.API.Services.Implementations
{
    public class WorkingHourService : IWorkingHourService
    {
        private readonly IWorkingHourRepository _workingHourRepository;

        public WorkingHourService(IWorkingHourRepository workingHourRepository)
        {
            _workingHourRepository = workingHourRepository;
        }

        public async Task<IEnumerable<WorkingHour>> GetByBarberIdAsync(int barberId)
        {
            return await _workingHourRepository.GetByBarberIdAsync(barberId);
        }

        public async Task AddAsync(WorkingHour workingHour)
        {
            await _workingHourRepository.AddAsync(workingHour);
        }

        public async Task UpdateAsync(WorkingHour workingHour)
        {
            await _workingHourRepository.UpdateAsync(workingHour);
        }

        public async Task DeleteAsync(WorkingHour workingHour)
        {
            await _workingHourRepository.DeleteAsync(workingHour);
        }

        public async Task AddDtoAsync(WorkingHourDto dto)
        {
            var workingHour = new WorkingHour
            {
                BarberId = dto.BarberId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Notes = dto.Notes
            };

            await _workingHourRepository.AddAsync(workingHour);
        }

        public async Task UpdateDtoAsync(WorkingHourDto dto)
        {
            var workingHour = new WorkingHour
            {
                Id = dto.Id,
                BarberId = dto.BarberId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Notes = dto.Notes
            };

            await _workingHourRepository.UpdateAsync(workingHour);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var workingHour = new WorkingHour { Id = id };
            await _workingHourRepository.DeleteAsync(workingHour);
        }

        public async Task AddDefaultWorkingHoursForTomorrowIfMissing()
        {
            var tomorrow = DateTime.Today.AddDays(1).DayOfWeek;

            var allBarbers = await _workingHourRepository.GetAllBarbersAsync();

            foreach (var barber in allBarbers)
            {
                var hasTomorrowHours = await _workingHourRepository.ExistsAsync(barber.Id, tomorrow);

                if (!hasTomorrowHours && tomorrow != DayOfWeek.Sunday)
                {
                    var defaultWorkingHour = new WorkingHour
                    {
                        BarberId = barber.Id,
                        DayOfWeek = tomorrow,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(18, 0, 0),
                        Notes = "Otomatik eklenen standart çalışma saati"
                    };

                    await _workingHourRepository.AddAsync(defaultWorkingHour);
                }
            }
        }
    }
}
