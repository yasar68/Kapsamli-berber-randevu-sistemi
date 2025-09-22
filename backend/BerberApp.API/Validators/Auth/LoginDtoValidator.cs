using FluentValidation;
using BerberApp.API.DTOs.Auth;

namespace BerberApp.API.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email alanı zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre alanı zorunludur.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                // İstersen ek kurallar ekleyebilirsin:
                .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.");
        }
    }
}
