namespace BerberApp.API.DTOs.Review
{
    public class CreateReviewDto
    {
        public int UserId { get; set; }            // Yorumu yapan müşterinin Id'si
        public int Rating { get; set; }             // Puanlama (örn: 1-5 arası)
        public string Comment { get; set; } = string.Empty;  // Yorum metni
    }
}
