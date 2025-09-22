using System;

namespace BerberApp.API.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }               // Kullanıcı ID'si
        public string FullName { get; set; } = string.Empty;  // Kullanıcı tam adı
        public string Email { get; set; } = string.Empty;     // E-posta
        public string? PhoneNumber { get; set; }               // Telefon numarası (opsiyonel)
        public DateTime CreatedAt { get; set; }                // Hesap oluşturulma tarihi
        public DateTime? LastLoginAt { get; set; }             // Son giriş zamanı (opsiyonel)
        public string? Role { get; set; }
    }
}
