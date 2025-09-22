namespace BerberApp.API.Config
{
    /// <summary>
    /// Rate limiting (istek sınırlandırma) ayarları
    /// </summary>
    public class RateLimitSettings
    {
        /// <summary>
        /// Bir IP adresinin belirli zaman diliminde yapabileceği maksimum istek sayısı
        /// </summary>
        public int MaxRequests { get; set; } = 100;

        /// <summary>
        /// İstek sayısının sıfırlanacağı süre (saniye cinsinden)
        /// Örneğin 60 saniye = 1 dakika
        /// </summary>
        public int TimeWindowSeconds { get; set; } = 60;

        /// <summary>
        /// Rate limit aşıldığında döndürülecek HTTP durum kodu (default 429 Too Many Requests)
        /// </summary>
        public int StatusCode { get; set; } = 429;

        /// <summary>
        /// Limit aşımı durumunda döndürülecek mesaj
        /// </summary>
        public string Message { get; set; } = "Çok fazla istek gönderildi. Lütfen biraz sonra tekrar deneyin.";
    }
}
