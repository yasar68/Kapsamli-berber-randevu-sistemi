namespace BerberApp.API.Config
{
    public class AppConfig
    {
        /// <summary>
        /// Uygulama adı, log ve hata mesajlarında kullanılabilir.
        /// </summary>
        public string ApplicationName { get; set; } = "BerberApp.API";

        /// <summary>
        /// Uygulamanın versiyonu, Swagger veya hata raporlarında gösterilebilir.
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Çalışma ortamı (Development, Staging, Production)
        /// </summary>
        public string Environment { get; set; } = "Development";

        /// <summary>
        /// Token geçerlilik süresi dakika cinsinden
        /// </summary>
        public int JwtTokenExpirationMinutes { get; set; } = 60;

        /// <summary>
        /// API için genel istek timeout süresi (saniye cinsinden)
        /// </summary>
        public int RequestTimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Maksimum kayıt sayısı, pagination için kullanılabilir.
        /// </summary>
        public int MaxPageSize { get; set; } = 50;

        /// <summary>
        /// Varsayılan sayfa numarası, pagination için.
        /// </summary>
        public int DefaultPageNumber { get; set; } = 1;

        /// <summary>
        /// Rate limiting için örnek: 1 dakika içinde izin verilen maksimum istek sayısı.
        /// </summary>
        public int RateLimitMaxRequestsPerMinute { get; set; } = 60;

        /// <summary>
        /// E-posta gönderen adres (örn: info@berberapp.com)
        /// </summary>
        public string DefaultEmailSender { get; set; } = "no-reply@berberapp.com";

        /// <summary>
        /// Dosya yükleme için maksimum dosya boyutu (byte cinsinden), örneğin 5MB
        /// </summary>
        public long MaxFileUploadSizeBytes { get; set; } = 5 * 1024 * 1024;

        /// <summary>
        /// Loglama için minimum seviye (Trace, Debug, Information, Warning, Error, Critical)
        /// </summary>
        public string LogMinimumLevel { get; set; } = "Information";

        /// <summary>
        /// Uygulama url’si, Swagger veya mail içi linklerde kullanılabilir.
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.berberapp.com";

        /// <summary>
        /// Veri tabanı bağlantı deneme sayısı (Retry)
        /// </summary>
        public int DbConnectionRetryCount { get; set; } = 3;

        /// <summary>
        /// Cache süresi dakika cinsinden, örn. 60 dakika
        /// </summary>
        public int CacheDurationMinutes { get; set; } = 60;

        // İstersen burada daha fazla global ayar tutabilirsin.
    }
}
