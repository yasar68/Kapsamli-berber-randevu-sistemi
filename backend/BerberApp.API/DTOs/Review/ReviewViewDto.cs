namespace BerberApp.API.DTOs.Review
{
    public class ReviewViewDto
    {
        public int Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}