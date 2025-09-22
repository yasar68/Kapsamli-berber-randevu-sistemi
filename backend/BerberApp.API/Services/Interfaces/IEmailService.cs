using System.Threading.Tasks;

namespace BerberApp.API.Services.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Belirtilen adrese e-posta gönderir.
        /// </summary>
        /// <param name="toEmail">Alıcının e-posta adresi</param>
        /// <param name="subject">E-posta konusu</param>
        /// <param name="body">E-posta içeriği (HTML olabilir)</param>
        /// <returns>Gönderme işleminin sonucu</returns>
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, byte[] attachmentBytes, string attachmentName);

    }
}
