namespace BerberApp.API.Models
{
    public class WorkingHour
    {
        public int Id { get; set; }

        public int BarberId { get; set; }

        public Barber? Barber { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string? Notes { get; set; }
    }
}
