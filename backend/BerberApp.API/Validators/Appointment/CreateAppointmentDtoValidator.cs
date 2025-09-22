using System;
using FluentValidation;
using BerberApp.API.DTOs.Appointment;

namespace BerberApp.API.Validators
{
    public class CreateAppointmentDtoValidator : AbstractValidator<CreateAppointmentDto>
    {
        public CreateAppointmentDtoValidator()
        {
            RuleFor(x => x.ServiceIds)
                .NotEmpty().WithMessage("En az bir servis seçilmelidir.")
                .Must(list => list != null && list.All(id => id > 0))
                .WithMessage("Tüm servis ID'leri sıfırdan büyük olmalıdır.");

            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Müşteri ID'si sıfırdan büyük olmalıdır.");

            RuleFor(x => x.BarberId)
                .GreaterThan(0).WithMessage("Berber ID'si sıfırdan büyük olmalıdır.");

            RuleFor(x => x.StartTime)
                .Must(BeInFuture).WithMessage("Randevu başlangıç zamanı gelecekte olmalıdır.");

            RuleFor(x => x.Note)
                .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olabilir.");
        }

        private bool BeInFuture(DateTime date)
        {
            return date > DateTime.UtcNow;
        }
    }
}
