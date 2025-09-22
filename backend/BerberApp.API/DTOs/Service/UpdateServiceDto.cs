using System.ComponentModel.DataAnnotations;

namespace BerberApp.API.DTOs.Service
{
    /// <summary>
    /// Mevcut bir hizmeti güncellemek için kullanılan DTO
    /// </summary>
    public class UpdateServiceDto
    {
        [Required]
        public int Id { get; set; }  // Güncellenecek hizmetin ID’si

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;  // Hizmet adı

        [MaxLength(500)]
        public string? Description { get; set; }          // Açıklama (opsiyonel)

        [Range(0, 10000)]
        public decimal Price { get; set; }                // Hizmet ücreti

        [Range(1, 300)]
        public int DurationMinutes { get; set; }          // Süresi (dakika cinsinden)

        public bool IsActive { get; set; }                // Aktif/pasif durumu
    }
}
