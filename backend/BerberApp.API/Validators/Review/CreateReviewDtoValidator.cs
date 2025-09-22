using FluentValidation;
using BerberApp.API.DTOs.Review;

namespace BerberApp.API.Validators
{
    public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Geçerli bir kullanıcı ID'si girilmelidir.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Puanlama 1 ile 5 arasında olmalıdır.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Yorum metni boş olamaz.")
                .MaximumLength(500).WithMessage("Yorum metni en fazla 500 karakter olabilir.");
        }
    }
}
