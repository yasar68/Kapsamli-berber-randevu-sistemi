using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BerberApp.API.Config;
using BerberApp.API.DTOs.Auth;
using BerberApp.API.Helpers;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using BerberApp.API.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace BerberApp.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly JwtHelper _jwtHelper;

        private readonly IUserRoleRepository _userRoleRepository;

        public AuthService(
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,   // <-- bunu ekle
            IRefreshTokenService refreshTokenService,
            IConfiguration configuration,
            ILogger<JwtHelper> logger)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;  // <-- bunu atama yap
            _refreshTokenService = refreshTokenService;

            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
            {
                throw new Exception("JWT ayarları yapılandırılmamış.");
            }

            _jwtHelper = new JwtHelper(Options.Create(jwtSettings), logger);
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Email ile kullanıcı kontrolü
            var existingUserByEmail = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUserByEmail != null)
                return new AuthResponseDto { Success = false, Message = "Bu email zaten kayıtlı." };

            // Telefon numarası ile kullanıcı kontrolü
            var existingUserByPhone = await _userRepository.GetByPhoneNumberAsync(registerDto.PhoneNumber);
            if (existingUserByPhone != null)
                return new AuthResponseDto { Success = false, Message = "Bu telefon numarası zaten kayıtlı." };

            // Şifre hash ve salt oluşturma
            CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                UserName = registerDto.Email,
                FullName = $"{registerDto.FirstName} {registerDto.LastName}",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            

            var token = _jwtHelper.GenerateToken(user.Id.ToString(), user.Email);

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                Message = "Kayıt başarılı."
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || !VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
                return new AuthResponseDto { Success = false, Message = "Email veya şifre hatalı." };

            // Kullanıcının rollerini al (UserRoleRepository veya IUserRoleRepository aracılığıyla)
            var roles = await _userRoleRepository.GetByUserIdAsync(user.Id);
            var roleNames = roles.Select(r => r.Role?.Name ?? "").ToArray();

            // Token üretimi, rolleri de gönder
            var accessToken = _jwtHelper.GenerateToken(user.Id.ToString(), user.Email, roleNames);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = "::1" // Gerçek IP'yi controller'dan alıp parametre olarak verebilirsin
            };

            await _refreshTokenService.AddAsync(refreshToken);

            return new AuthResponseDto
            {
                Success = true,
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                Message = "Giriş başarılı."
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string oldToken)
        {
            var refreshToken = await _refreshTokenService.GetByTokenAsync(oldToken);

            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.Expires < DateTime.UtcNow)
                throw new Exception("Geçersiz veya süresi dolmuş refresh token.");

            var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
            if (user == null) throw new Exception("Kullanıcı bulunamadı.");

            var newAccessToken = _jwtHelper.GenerateToken(user.Id.ToString(), user.Email);
            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = "::1"
            };

            await _refreshTokenService.ReplaceTokenAsync(refreshToken, newRefreshToken.Token, "::1");
            await _refreshTokenService.AddAsync(newRefreshToken);

            return new AuthResponseDto
            {
                Success = true,
                Token = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                Message = "Token yenilendi."
            };
        }


        public async Task LogoutAsync(int userId)
        {
            await _refreshTokenService.DeleteByUserIdAsync(userId);
        }

        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null && VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
}
