using System;
using FluentValidation;
using BerberApp.API.DTOs.Appointment;

namespace BerberApp.API.Validators
{
    public class UpdateAppointmentDtoValidator : AbstractValidator<UpdateAppointmentDto>
    {
        public UpdateAppointmentDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Geçerli bir Id girilmelidir.");

            RuleFor(x => x.BarberId)
                .GreaterThan(0).WithMessage("Berber Id sıfırdan büyük olmalıdır.");

            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("Hizmet Id sıfırdan büyük olmalıdır.");

            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Müşteri Id sıfırdan büyük olmalıdır.");

            RuleFor(x => x.StartTime)
                .Must(BeInFutureOrPresent)
                .WithMessage("Başlangıç zamanı geçmişte olamaz.");

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .WithMessage("Bitiş zamanı başlangıçtan sonra olmalıdır.");

            RuleFor(x => x.Note)
                .MaximumLength(500)
                .WithMessage("Notlar en fazla 500 karakter olabilir.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Durum 'Pending', 'Confirmed', 'Cancelled' veya 'Completed' olabilir.");

        }

        private bool BeInFutureOrPresent(DateTime date)
        {
            return date >= DateTime.Now;
        }

        private bool BeValidStatus(string status)
        {
            return status == "Bekliyor" || status == "Onaylandı" || status == "İptal Edildi" || status == "Tamamlandı";
        }
    }
}
