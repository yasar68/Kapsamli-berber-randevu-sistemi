using BerberApp.API.Models;

namespace BerberApp.API.Repositories.Interfaces
{
    public interface IAppointmentServiceRepository
    {
        Task AddAsync(AppointmentService appointmentService);
        Task<IEnumerable<AppointmentService>> GetByAppointmentIdAsync(int appointmentId);
        Task DeleteAsync(AppointmentService appointmentService);
        // İhtiyaç varsa diğer CRUD metotları
    }

}