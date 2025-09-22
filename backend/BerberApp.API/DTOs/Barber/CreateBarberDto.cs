namespace BerberApp.API.DTOs.Barber
{
    public class CreateBarberDto
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required List<string> Specialties { get; set; }
    }
}