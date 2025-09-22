using System;

namespace BerberApp.API.DTOs.WorkingHour
{
    public class WorkingHourDto
    {
        public int Id { get; set; }
        public int BarberId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Notes { get; set; }
    }
}
