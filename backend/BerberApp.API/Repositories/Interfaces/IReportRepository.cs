using System.Threading.Tasks;
using BerberApp.API.Models;

namespace BerberApp.API.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task AddAsync(Report report);
        Task<Report?> GetByIdAsync(int id);
        Task UpdateAsync(Report report);
        Task DeleteAsync(Report report);
    }
}
