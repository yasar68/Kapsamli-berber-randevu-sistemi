using System;
using System.Threading.Tasks;
using BerberApp.API.DTOs.Report;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using BerberApp.API.Services.Interfaces;

namespace BerberApp.API.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IReportRepository _reportRepository;

        public ReportService(IAppointmentRepository appointmentRepository, IReportRepository reportRepository)
        {
            _appointmentRepository = appointmentRepository;
            _reportRepository = reportRepository;
        }

        public async Task<Report> GenerateReportAsync(ReportRequestDto requestDto)
        {
            if (!requestDto.StartDate.HasValue || !requestDto.EndDate.HasValue)
                throw new ArgumentException("StartDate ve EndDate zorunludur.");

            var startDate = requestDto.StartDate.Value;
            var endDate = requestDto.EndDate.Value;

            var appointments = await _appointmentRepository.GetAppointmentsBetweenDatesAsync(
                startDate,
                endDate,
                requestDto.BarberId > 0 ? requestDto.BarberId : null);

            var appointmentList = appointments.ToList();

            return new Report
            {
                BarberId = requestDto.BarberId,
                StartDate = startDate,
                EndDate = endDate,
                TotalAppointments = appointmentList.Count,
                PendingAppointments = appointmentList.Count(a => a.Status == AppointmentStatus.Pending),
                ConfirmedAppointments = appointmentList.Count(a => a.Status == AppointmentStatus.Confirmed),
                CompletedAppointments = appointmentList.Count(a => a.Status == AppointmentStatus.Completed),
                CancelledAppointments = appointmentList.Count(a => a.Status == AppointmentStatus.Cancelled),
                TotalRevenue = appointmentList.Sum(a => a.Price), // Tüm randevuların toplam geliri
                CompletedRevenue = appointmentList
                    .Where(a => a.Status == AppointmentStatus.Completed)
                    .Sum(a => a.Price),
                ConfirmedRevenue = appointmentList
                    .Where(a => a.Status == AppointmentStatus.Confirmed)
                    .Sum(a => a.Price),
                Notes = requestDto.Notes
            };
        }

        public async Task<Report?> GetReportByIdAsync(int reportId)
        {
            return await _reportRepository.GetByIdAsync(reportId);
        }

        public async Task DeleteReportAsync(int reportId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report != null)
            {
                await _reportRepository.DeleteAsync(report);
            }
        }

        public async Task UpdateReportAsync(Report report)
        {
            await _reportRepository.UpdateAsync(report);
        }
    }
}
