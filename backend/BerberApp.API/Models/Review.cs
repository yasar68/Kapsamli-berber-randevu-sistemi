namespace BerberApp.API.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; } // Yeni eklenen property

        // Foreign keys
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}