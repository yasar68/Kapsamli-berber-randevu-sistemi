using System.ComponentModel.DataAnnotations;

namespace BerberApp.API.DTOs.Service
{
    /// <summary>
    /// Yeni bir hizmet oluşturmak için kullanılan DTO
    /// </summary>
    public class CreateServiceDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;  // Hizmet adı

        [MaxLength(500)]
        public string? Description { get; set; }          // Açıklama (opsiyonel)

        [Range(0, 10000)]
        public decimal Price { get; set; }                // Hizmet ücreti

        [Range(1, 300)]
        public int DurationMinutes { get; set; }          // Süresi (dakika cinsinden)

        public bool IsActive { get; set; } = true;        // Varsayılan olarak aktif
    }
}
