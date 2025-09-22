using System;

namespace BerberApp.API.Models
{
    public class Service
    {
        public int Id { get; set; }                    // Servisin benzersiz ID'si

        public string Name { get; set; } = null!;      // Servis adı

        public string? Description { get; set; }       // Servis açıklaması

        public decimal Price { get; set; }             // Servis ücreti

        public int DurationMinutes { get; set; }       // DTO ve ServiceService.cs ile uyumlu isim

        public bool IsActive { get; set; }             // Servis aktif mi?

        public DateTime CreatedAt { get; set; }        // Oluşturulma tarihi

        public DateTime? UpdatedAt { get; set; }       // Güncellenme tarihi (opsiyonel)
        public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }
}
