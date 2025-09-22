using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.Models;

namespace BerberApp.API.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        // Temel CRUD Operasyonları
        Task<Appointment?> GetByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task AddAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task DeleteAsync(Appointment appointment);
        Task<bool> ExistsAsync(int id);

        // Özel Sorgular
        Task<IEnumerable<Appointment>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Appointment>> GetByBarberIdAsync(int barberId);

        // Tarih Aralığı Sorgusu
        Task<IEnumerable<Appointment>> GetAppointmentsBetweenDatesAsync(
            DateTime start,
            DateTime end,
            int? barberId = null);

        // Durum Bazlı Sorgular
        Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status);
        Task<IEnumerable<Appointment>> GetByStatusAndDateRangeAsync(
            AppointmentStatus status,
            DateTime startDate,
            DateTime endDate,
            int? barberId = null);
        Task<IEnumerable<Appointment>> GetByMultipleStatusesAsync(
            IEnumerable<AppointmentStatus> statuses,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? barberId = null);

        // İstatistiksel Sorgular
        Task<int> GetAppointmentCountByBarberAsync(int barberId);
        Task<decimal> GetTotalRevenueByBarberAsync(int barberId);
        Task<decimal> GetRevenueByStatusAsync(int barberId, AppointmentStatus status);
        Task<DateTime?> GetLastAppointmentDateAsync(int userId);
        Task DeleteAllForUserAsync(int userId);
        Task<IEnumerable<Appointment>> GetAppointmentsAsync(int? customerId, int? barberId, AppointmentStatus? status);
        Task<IEnumerable<Appointment>> GetAppointmentsByBarberAndDateAsync(int barberId, DateTime date);
        Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int? barberId = null);
        
    }
}