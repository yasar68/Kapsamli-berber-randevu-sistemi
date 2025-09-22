namespace BerberApp.API.DTOs.Review
{
    public class UpdateReviewDto
    {
        public int ReviewId { get; set; }          // Güncellenecek yorumun Id'si
        public int Rating { get; set; }             // Yeni puanlama (örn: 1-5 arası)
        public string Comment { get; set; } = string.Empty;  // Yeni yorum metni
    }
}
