using System;

namespace BerberApp.API.DTOs.Appointment
{
    public class AppointmentViewDto
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string ServiceName { get; set; } = string.Empty;

        public string BarberName { get; set; } = string.Empty;

        public string CustomerName { get; set; } = "Misafir"; // Varsayılan değer

        public string Status { get; set; } = string.Empty; // örn: "Bekliyor", "Onaylandı", "İptal Edildi"

        public List<string> ServiceNames { get; set; } = new List<string>();  // Burayı ekle

        public decimal Price { get; set; }
        public string? Notes { get; set; }
    }
}
