using System.ComponentModel.DataAnnotations;

namespace BerberApp.API.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "İsim alanı zorunludur.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "İsim 2 ile 50 karakter arasında olmalıdır.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyisim alanı zorunludur.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Soyisim 2 ile 50 karakter arasında olmalıdır.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
