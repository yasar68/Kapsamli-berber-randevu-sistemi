using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BerberApp.API.DTOs.User;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using BerberApp.API.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace BerberApp.API.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;

        // Şifre sıfırlama kodlarını geçici saklamak için thread-safe dictionary
        private static ConcurrentDictionary<string, string> _passwordResetCodes = new();

        public UserService(
            IUserRepository userRepository,
            IAppointmentRepository appointmentRepository, IEmailService emailService,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _appointmentRepository = appointmentRepository;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return MapToUserDto(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToUserDto);
        }

        public async Task AddUserAsync(CreateUserDto createUserDto)
        {
            if (createUserDto == null)
                throw new ArgumentNullException(nameof(createUserDto));

            using var hmac = new HMACSHA512();

            var user = new User
            {
                FullName = createUserDto.FullName,
                Email = createUserDto.Email ?? string.Empty,
                PhoneNumber = createUserDto.PhoneNumber,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(createUserDto.Password)),
                PasswordSalt = hmac.Key,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
        }

        public async Task UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            if (updateUserDto == null)
                throw new ArgumentNullException(nameof(updateUserDto));

            var user = await _userRepository.GetByIdAsync(updateUserDto.Id);
            if (user == null)
                throw new KeyNotFoundException($"User with Id {updateUserDto.Id} not found.");

            user.FullName = updateUserDto.FullName;

            if (!string.IsNullOrWhiteSpace(updateUserDto.Email))
                user.Email = updateUserDto.Email;

            if (!string.IsNullOrWhiteSpace(updateUserDto.PhoneNumber))
                user.PhoneNumber = updateUserDto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
            {
                using var hmac = new HMACSHA512();
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(updateUserDto.Password));
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<ApiResponse> DeleteUserAsync(int id)
        {
            var transaction = await _userRepository.BeginTransactionAsync();

            try
            {
                await _appointmentRepository.DeleteAllForUserAsync(id);
                await _userRepository.HardDeleteAsync(id);

                await transaction.CommitAsync();

                return new ApiResponse
                {
                    Success = true,
                    Message = "Kullanıcı ve ilişkili veriler başarıyla silindi"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Kullanıcı silinemedi. ID: {id}");

                return new ApiResponse
                {
                    Success = false,
                    Message = "Kullanıcı silinirken bir hata oluştu"
                };
            }
        }

        // Şifre sıfırlama kodu oluştur ve e-posta ile gönder (mail gönderim kodunu kendin yazmalısın)
        public async Task<ApiResponse> SendPasswordResetCodeAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Kullanıcı bulunamadı."
                };
            }

            // Rastgele 6 haneli kod üret
            var code = GenerateRandomCode(6);

            // Kod ve e-posta eşlemesini dictionary'ye ekle / güncelle
            _passwordResetCodes.AddOrUpdate(email, code, (key, oldValue) => code);

            // ✅ Kod e-postayla gönderiliyor
            await _emailService.SendEmailAsync(
                email,
                "BerberApp Şifre Sıfırlama",
                $"Merhaba {user.FullName},<br><br><b>{code}</b> kodunu kullanarak şifrenizi sıfırlayabilirsiniz.<br><br>Bu kod 10 dakika içinde geçerlidir.<br><br>İyi günler dileriz.<br>BerberApp Ekibi"
            );

            _logger.LogInformation($"Şifre sıfırlama kodu gönderildi: {email} - Kod: {code}");

            return new ApiResponse
            {
                Success = true,
                Message = "Şifre sıfırlama kodu gönderildi."
            };
        }

        // Şifre sıfırlama işlemi (kod doğrulaması ve şifre güncelleme)
        public async Task<ApiResponse> ResetPasswordAsync(string email, string code, string newPassword)
        {
            if (!_passwordResetCodes.TryGetValue(email, out var savedCode) || savedCode != code)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Geçersiz veya süresi dolmuş kod."
                };
            }

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Kullanıcı bulunamadı."
                };
            }

            using var hmac = new HMACSHA512();
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            // Kullanılan kodu kaldır
            _passwordResetCodes.TryRemove(email, out _);

            return new ApiResponse
            {
                Success = true,
                Message = "Şifre başarıyla sıfırlandı."
            };
        }

        // Yardımcı metotlar

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                LastLoginAt = null
            };
        }

        private string GenerateRandomCode(int length)
        {
            var random = new Random();
            var code = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                code.Append(random.Next(0, 10)); // 0-9 arası rakam
            }
            return code.ToString();
        }
    }
}
