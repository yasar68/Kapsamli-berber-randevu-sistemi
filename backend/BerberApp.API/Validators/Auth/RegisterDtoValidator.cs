using FluentValidation;
using BerberApp.API.DTOs.Auth;

namespace BerberApp.API.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("İsim alanı zorunludur.")
                .Length(2, 50).WithMessage("İsim 2 ile 50 karakter arasında olmalıdır.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyisim alanı zorunludur.")
                .Length(2, 50).WithMessage("Soyisim 2 ile 50 karakter arasında olmalıdır.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email alanı zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre alanı zorunludur.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
                .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
                .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az bir özel karakter içermelidir.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon numarası zorunludur.")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Geçerli bir telefon numarası giriniz.");
        }
    }
}
