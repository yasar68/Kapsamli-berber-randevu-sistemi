using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.Models;

namespace BerberApp.API.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId);
        Task AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task DeleteAsync(RefreshToken refreshToken);
        Task DeleteByUserIdAsync(int userId);

        // ✅ Yeni eklenen gelişmiş metotlar:
        Task RevokeTokenAsync(RefreshToken token, string ipAddress);
        Task ReplaceTokenAsync(RefreshToken oldToken, string newToken, string ipAddress);
    }
}
