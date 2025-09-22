using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BerberApp.API.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BerberApp.API.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? throw new InvalidOperationException("SMTP host config missing");
            var smtpPortString = _configuration["EmailSettings:SmtpPort"] ?? throw new InvalidOperationException("SMTP port config missing");
            int smtpPort = int.Parse(smtpPortString);
            var smtpUser = _configuration["EmailSettings:SmtpUsername"] ?? throw new InvalidOperationException("SMTP user config missing");
            var smtpPass = _configuration["EmailSettings:SmtpPassword"] ?? throw new InvalidOperationException("SMTP password config missing");
            var fromEmail = _configuration["EmailSettings:FromAddress"] ?? throw new InvalidOperationException("From email config missing");
            var fromName = _configuration["EmailSettings:FromName"] ?? "BerberApp";

            using var message = new MailMessage();
            message.From = new MailAddress(fromEmail, fromName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
            };

            await client.SendMailAsync(message);
        }
        public async Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, byte[] attachmentBytes, string attachmentName)
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? throw new InvalidOperationException("SMTP host config missing");
            var smtpPortString = _configuration["EmailSettings:SmtpPort"] ?? throw new InvalidOperationException("SMTP port config missing");
            int smtpPort = int.Parse(smtpPortString);
            var smtpUser = _configuration["EmailSettings:SmtpUsername"] ?? throw new InvalidOperationException("SMTP user config missing");
            var smtpPass = _configuration["EmailSettings:SmtpPassword"] ?? throw new InvalidOperationException("SMTP password config missing");
            var fromEmail = _configuration["EmailSettings:FromAddress"] ?? throw new InvalidOperationException("From email config missing");
            var fromName = _configuration["EmailSettings:FromName"] ?? "BerberApp";

            using var message = new MailMessage();
            message.From = new MailAddress(fromEmail, fromName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var ms = new MemoryStream(attachmentBytes);
            message.Attachments.Add(new Attachment(ms, attachmentName));

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
            };

            await client.SendMailAsync(message);
        }
    }
}
