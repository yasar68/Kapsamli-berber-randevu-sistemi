using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BerberApp.API.Services.Interfaces;
using BerberApp.API.Models;
using Microsoft.EntityFrameworkCore;
using BerberApp.API.Data;

namespace BerberApp.API.Services.Implementations
{
    public class AppointmentReminderService : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppointmentReminderService> _logger;

        public AppointmentReminderService(IServiceProvider serviceProvider, ILogger<AppointmentReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AppointmentReminderService başlatıldı.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAppointmentsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Randevu hatırlatma kontrolü sırasında hata oluştu.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task CheckAppointmentsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var now = DateTime.UtcNow;
            var targetTime = now.AddMinutes(30);
            _logger.LogInformation($"Reminder check - now: {now}, targetTime: {targetTime}");

            var appointments = await dbContext.Appointments
                .Include(a => a.User)
                .Where(a =>
                    a.AppointmentDate > now &&
                    a.AppointmentDate <= targetTime &&
                    a.Status == AppointmentStatus.Confirmed &&
                    a.ReminderSent == false)
                .ToListAsync();

            foreach (var appointment in appointments)
            {
                var user = appointment.User;

                if (string.IsNullOrEmpty(user.Email))
                    continue;

                var subject = "Randevunuz 30 dakika içinde başlıyor!";
                var body = $"""
                    Merhaba {user.FullName},<br/>
                    {appointment.AppointmentDate:HH:mm} saatindeki randevunuza 30 dakika kaldı.<br/>
                    Unutmayın, geç kalmamak önemli :)<br/><br/>
                    BerberApp
                """;

                try
                {
                    await emailService.SendEmailAsync(user.Email, subject, body);
                    appointment.ReminderSent = true;
                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation($"Hatırlatma maili gönderildi: {user.Email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Mail gönderim hatası: {user.Email}");
                }
            }
        }
    }
}
