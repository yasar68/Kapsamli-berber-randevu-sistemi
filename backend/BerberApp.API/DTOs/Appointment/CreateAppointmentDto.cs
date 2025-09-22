using System;
using System.ComponentModel.DataAnnotations;
using BerberApp.API.ValidationAttributes;

namespace BerberApp.API.DTOs.Appointment
{
    public class CreateAppointmentDto
    {
        [Required(ErrorMessage = "Servis ID zorunludur")]
        public List<int> ServiceIds { get; set; } = new();

        [Required(ErrorMessage = "Müşteri ID zorunludur")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Berber ID zorunludur")]
        public int BarberId { get; set; } // Hem DTO'da hem de serviste kullanılacak

        [Required(ErrorMessage = "Başlangıç saati zorunludur")]
        [FutureDate(ErrorMessage = "Geçmiş tarihli randevu oluşturulamaz")]
        public DateTime StartTime { get; set; }
        public string? Note { get; set; }
        public bool EmailBildirimiGonder { get; set; } = false;
    }
}