using System;
using System.ComponentModel.DataAnnotations;
using BerberApp.API.Models;
using System.Text.Json.Serialization;

namespace BerberApp.API.DTOs.Appointment
{
    public class UpdateAppointmentDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int BarberId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string? Note { get; set; }
        
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public bool SendEmailNotification { get; set; } = false;
    }
}
