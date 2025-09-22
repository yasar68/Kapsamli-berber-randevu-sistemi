using System;

namespace BerberApp.API.Config
{
    /// <summary>
    /// Veritabanı bağlantı ayarları ve ilgili konfigürasyonlar.
    /// </summary>
    public class DatabaseSettings
    {
        /// <summary>
        /// Veritabanı bağlantı stringi. (Zorunlu)
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Veritabanı sağlayıcısı (PostgreSQL, SqlServer, MySql, Sqlite vb.)
        /// </summary>
        public string Provider { get; set; } = "PostgreSQL";

        /// <summary>
        /// Maksimum bağlantı havuzu boyutu
        /// </summary>
        public int MaxPoolSize { get; set; } = 100;

        /// <summary>
        /// Minimum bağlantı havuzu boyutu
        /// </summary>
        public int MinPoolSize { get; set; } = 5;

        /// <summary>
        /// Bağlantı zaman aşımı (saniye)
        /// </summary>
        public int ConnectionTimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Komut zaman aşımı (saniye), EF Core için de önemli
        /// </summary>
        public int CommandTimeoutSeconds { get; set; } = 60;

        /// <summary>
        /// Veritabanına bağlanırken yeniden deneme sayısı
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Bağlantı yeniden deneme gecikmesi (ms)
        /// </summary>
        public int RetryDelayMilliseconds { get; set; } = 500;

        /// <summary>
        /// SQL sorguları için loglama aç/kapat (performans için isteğe bağlı)
        /// </summary>
        public bool EnableSqlLogging { get; set; } = true;

        /// <summary>
        /// Migration otomatik uygulanacak mı?
        /// </summary>
        public bool AutoMigrate { get; set; } = true;

        /// <summary>
        /// Seed işlemi otomatik yapılacak mı?
        /// </summary>
        public bool AutoSeed { get; set; } = true;

        /// <summary>
        /// Transaction zaman aşımı (saniye)
        /// </summary>
        public int TransactionTimeoutSeconds { get; set; } = 120;

        /// <summary>
        /// Debug modda veritabanı detay logu tutulacak mı
        /// </summary>
        public bool EnableDetailedDebugLogging { get; set; } = false;

        /// <summary>
        /// Connection string içindeki hassas bilgileri (şifre vb.) konsola yazmasın mı? (Güvenlik)
        /// </summary>
        public bool MaskSensitiveDataInLogs { get; set; } = true;
    }
}
