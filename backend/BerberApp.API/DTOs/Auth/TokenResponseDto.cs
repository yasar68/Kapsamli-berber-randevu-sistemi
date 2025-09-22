namespace BerberApp.API.DTOs.Auth
{
    public class TokenResponseDto
    {
        /// <summary>
        /// Giriş sonrası kullanılacak JWT access token
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Opsiyonel: Token süresi dolduğunda yeni token almak için kullanılan refresh token
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Access token'ın süresi (dakika cinsinden)
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Kullanıcının adı ve soyadı
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının email adresi
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// İsteğe bağlı kullanıcı ID'si
        /// </summary>
        public int UserId { get; set; }
    }
}
