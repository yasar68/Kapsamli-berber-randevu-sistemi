namespace BerberApp.API.Config
{
    /// <summary>
    /// E-posta gönderim ayarları
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// SMTP sunucu adresi (ör. smtp.gmail.com)
        /// </summary>
        public string SmtpHost { get; set; } = string.Empty;

        /// <summary>
        /// SMTP portu (genellikle 587 veya 465)
        /// </summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// SMTP sunucusu için kullanıcı adı (genellikle e-posta adresi)
        /// </summary>
        public string SmtpUsername { get; set; } = string.Empty;

        /// <summary>
        /// SMTP sunucusu için parola
        /// </summary>
        public string SmtpPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gönderici e-posta adresi (örn: no-reply@berberapp.com)
        /// </summary>
        public string FromAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gönderici adı (örn: BerberApp Destek)
        /// </summary>
        public string FromName { get; set; } = "BerberApp";

        /// <summary>
        /// SSL/TLS kullanılsın mı?
        /// </summary>
        public bool EnableSsl { get; set; } = true;

        /// <summary>
        /// Bağlantı zaman aşımı (milisaniye)
        /// </summary>
        public int Timeout { get; set; } = 10000;
    }
}
