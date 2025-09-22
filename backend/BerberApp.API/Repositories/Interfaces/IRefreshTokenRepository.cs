using System.Threading.Tasks;
using BerberApp.API.Models;

namespace BerberApp.API.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);

        Task AddAsync(RefreshToken refreshToken);

        Task UpdateAsync(RefreshToken refreshToken);

        Task DeleteAsync(RefreshToken refreshToken);

        Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId);

    }
}
