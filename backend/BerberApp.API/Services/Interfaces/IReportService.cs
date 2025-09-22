using System.Threading.Tasks;
using BerberApp.API.DTOs.Report;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;

namespace BerberApp.API.Services.Interfaces
{
    public interface IReportService
    {
        Task<Report> GenerateReportAsync(ReportRequestDto requestDto);
        Task<Report?> GetReportByIdAsync(int reportId);
        Task DeleteReportAsync(int reportId);
        Task UpdateReportAsync(Report report);
    }
}
