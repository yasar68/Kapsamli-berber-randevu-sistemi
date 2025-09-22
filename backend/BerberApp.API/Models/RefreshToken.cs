using System;

namespace BerberApp.API.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }  // Birincil anahtar

        public string Token { get; set; } = null!;  // Refresh token string

        public DateTime Expires { get; set; }  // Token geçerlilik süresi sonu

        public bool IsRevoked { get; set; } = false;  // Token iptal edildi mi?

        public DateTime CreatedAt { get; set; }  // Oluşturulma zamanı

        public string? CreatedByIp { get; set; }  // Token oluşturulurken istek yapılan IP adresi

        public DateTime? RevokedAt { get; set; }  // Token iptal edilme zamanı (varsa)

        public string? RevokedByIp { get; set; }  // Token iptal edilirken kullanılan IP

        public string? ReplacedByToken { get; set; }  // Yenilenme sonrası verilen yeni token (varsa)

        // İlişkisel alan
        public int UserId { get; set; }  // Hangi kullanıcıya ait

        public User User { get; set; } = null!;
    }
}
