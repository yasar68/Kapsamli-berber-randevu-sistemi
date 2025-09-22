using System.ComponentModel.DataAnnotations;

namespace BerberApp.API.DTOs.User
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ad Soyad 2 ile 100 karakter arasında olmalıdır.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string? PhoneNumber { get; set; }
    }
}
