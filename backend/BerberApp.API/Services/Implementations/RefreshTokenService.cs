using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using BerberApp.API.Services.Interfaces;

namespace BerberApp.API.Services.Implementations
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _refreshTokenRepository.GetByTokenAsync(token);
        }

        public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId)
        {
            return await _refreshTokenRepository.GetByUserIdAsync(userId);
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _refreshTokenRepository.AddAsync(refreshToken);
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            await _refreshTokenRepository.UpdateAsync(refreshToken);
        }

        public async Task DeleteAsync(RefreshToken refreshToken)
        {
            await _refreshTokenRepository.DeleteAsync(refreshToken);
        }

        public async Task DeleteByUserIdAsync(int userId)
        {
            var tokens = await _refreshTokenRepository.GetByUserIdAsync(userId);
            foreach (var token in tokens)
            {
                await _refreshTokenRepository.DeleteAsync(token);
            }
        }

        // ✅ Token'ı güvenli şekilde iptal et
        public async Task RevokeTokenAsync(RefreshToken token, string ipAddress)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            await _refreshTokenRepository.UpdateAsync(token);
        }

        // ✅ Token yenilendiğinde eski token'a referans bırak
        public async Task ReplaceTokenAsync(RefreshToken oldToken, string newToken, string ipAddress)
        {
            oldToken.IsRevoked = true;
            oldToken.RevokedAt = DateTime.UtcNow;
            oldToken.RevokedByIp = ipAddress;
            oldToken.ReplacedByToken = newToken;
            await _refreshTokenRepository.UpdateAsync(oldToken);
        }
    }
}
