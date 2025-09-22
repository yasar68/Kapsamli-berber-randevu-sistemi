using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BerberApp.API.Models
{
    public class Barber
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public List<string> Specialties { get; set; } = new List<string>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Buraya WorkingHours koleksiyonunu ekle
        public ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();

        // Navigation Properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
