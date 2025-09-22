using System;

namespace BerberApp.API.DTOs.Service
{
    /// <summary>
    /// Hizmetin istemciye gösterilecek hali
    /// </summary>
    public class ServiceViewDto
    {
        public int Id { get; set; }                     // Hizmet ID'si
        public string Name { get; set; } = string.Empty; // Hizmet adı
        public string? Description { get; set; }         // Açıklama (opsiyonel)
        public decimal Price { get; set; }               // Fiyat
        public int DurationMinutes { get; set; }         // Tahmini süre
        public bool IsActive { get; set; }               // Aktif mi?
        public DateTime CreatedAt { get; set; }          // Oluşturulma tarihi
    }
}
