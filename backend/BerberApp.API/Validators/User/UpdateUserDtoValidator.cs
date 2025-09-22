using FluentValidation;
using BerberApp.API.DTOs.User;

namespace BerberApp.API.Validators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Geçerli bir kullanıcı ID'si girilmelidir.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Ad Soyad boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Ad Soyad maksimum 100 karakter olabilir.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Geçerli bir email adresi giriniz.")
                .MaximumLength(150).WithMessage("Email maksimum 150 karakter olabilir.");

            RuleFor(x => x.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .Matches(@"^\+?[0-9\s\-]{7,20}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Geçerli bir telefon numarası giriniz.")
                .MaximumLength(20).WithMessage("Telefon numarası maksimum 20 karakter olabilir.");

            RuleFor(x => x.Password)
                .MinimumLength(6).When(x => !string.IsNullOrEmpty(x.Password))
                .WithMessage("Şifre en az 6 karakter olmalıdır.");
        }
    }
}
