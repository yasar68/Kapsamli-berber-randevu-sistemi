using System.Threading.Tasks;
using BerberApp.API.DTOs.Auth;

namespace BerberApp.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task LogoutAsync(int userId);
        Task<bool> ValidateUserAsync(string email, string password);
        Task<AuthResponseDto> RefreshTokenAsync(string token);
    }
}
