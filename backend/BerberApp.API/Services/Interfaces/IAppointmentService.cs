using BerberApp.API.DTOs.Appointment;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BerberApp.API.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentViewDto>> GetAllAppointmentsAsync();
        Task<AppointmentViewDto?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<AppointmentViewDto>> GetAppointmentsByUserIdAsync(int userId);
        Task<IEnumerable<AppointmentViewDto>> GetAppointmentsByBarberIdAsync(int barberId);
        Task<AppointmentViewDto> CreateAppointmentAsync(CreateAppointmentDto dto);
        Task<AppointmentViewDto?> UpdateAppointmentAsync(UpdateAppointmentDto dto);
        Task<bool> DeleteAppointmentAsync(int id);
        Task<IEnumerable<AppointmentViewDto>> GetAppointmentsAsync(int? customerId, int? barberId, string? status);
        Task<IEnumerable<(TimeSpan Start, TimeSpan End)>> GetAvailableTimeSlotsAsync(int barberId, DateTime date);
        Task<IEnumerable<AppointmentViewDto>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int? barberId = null);

    }
}
