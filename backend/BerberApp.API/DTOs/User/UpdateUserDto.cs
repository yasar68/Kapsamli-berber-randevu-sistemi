using System.ComponentModel.DataAnnotations;

namespace BerberApp.API.DTOs.User
{
    public class UpdateUserDto
    {
        [Required]
        public int Id { get; set; }  // Güncellenecek kullanıcının ID'si (zorunlu)

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;  // Kullanıcının tam adı

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }  // E-posta (isteğe bağlı ama geçerli formatta olmalı)

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }  // Telefon numarası (isteğe bağlı)

        [MinLength(6)]
        public string? Password { get; set; }  // Yeni parola (opsiyonel, boşsa parola değişmez)

        // Eğer istersen başka güncellenebilir alanlar ekleyebilirsin
        // Örnek: adres, kullanıcı tipi vs.
    }
}
