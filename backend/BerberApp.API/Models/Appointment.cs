using System;
using BerberApp.API.Models;
using BerberApp.API.Services.Implementations;
namespace BerberApp.API.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BarberId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }
        public string? Notes { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal Price { get; set; }
        public bool ReminderSent { get; set; } = false;

        // Navigation properties
        public User User { get; set; } = null!;
        public Barber Barber { get; set; } = null!;

        // Çoktan çoğa ilişki için
        public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }

    public enum AppointmentStatus
    {
        Pending,     // Beklemede
        Confirmed,   // Onaylandı
        Cancelled,   // İptal edildi
        Completed    // Tamamlandı
    }
}
