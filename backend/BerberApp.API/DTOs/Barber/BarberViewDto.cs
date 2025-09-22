namespace BerberApp.API.DTOs.Barber
{
    public class BarberViewDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<string> Specialties { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
    }
}