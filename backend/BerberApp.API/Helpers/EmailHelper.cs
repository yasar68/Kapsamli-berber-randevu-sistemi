using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using BerberApp.API.Config;

namespace BerberApp.API.Helpers
{
    public class EmailHelper
    {
        private readonly EmailSettings _emailSettings;

        public EmailHelper(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = true)
        {
            try
            {
                using var smtpClient = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                    EnableSsl = _emailSettings.EnableSsl,
                    Timeout = _emailSettings.Timeout
                };

                var mailMessage = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.FromAddress, _emailSettings.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isBodyHtml
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);

                return true;
            }
            catch
            {
                // İstersen loglama burada yapılabilir
                return false;
            }
        }
    }
}
