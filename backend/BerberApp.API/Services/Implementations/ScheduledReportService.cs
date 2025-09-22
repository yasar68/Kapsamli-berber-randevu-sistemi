using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BerberApp.API.Services.Interfaces;
using BerberApp.API.DTOs.Report;  // Report DTO varsa
using BerberApp.API.DTOs.User;
using BerberApp.API.Models;       // AppointmentStatus ve Report modeli için
using BerberApp.API.Repositories.Interfaces;

// Alias tanımları çakışmaları önlemek için
using ModelService = BerberApp.API.Models.Service;
using HelperService = BerberApp.API.Helpers.Service;

using ModelAppointment = BerberApp.API.Models.Appointment;
using HelperAppointment = BerberApp.API.Helpers.Appointment;

using HelperCustomer = BerberApp.API.Helpers.Customer;

namespace BerberApp.API.Services.Implementations
{
    public class ScheduledReportService : BackgroundService
    {
        private readonly ILogger<ScheduledReportService> _logger;
        private readonly IReportService _reportService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IBarberService _barberService;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IServiceRepository _serviceRepository;

        public ScheduledReportService(
            ILogger<ScheduledReportService> logger,
            IReportService reportService,
            IUserService userService,
            IEmailService emailService,
            IBarberService barberService,
            IAppointmentRepository appointmentRepository,
            IServiceRepository serviceRepository)
        {
            _logger = logger;
            _reportService = reportService;
            _userService = userService;
            _emailService = emailService;
            _barberService = barberService;
            _appointmentRepository = appointmentRepository;
            _serviceRepository = serviceRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ScheduledReportService başladı.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;

                    if (now.Hour == 0 && now.Minute == 0)
                    {
                        await GenerateAndSendReportAsync(now.Date.AddDays(-1), now.Date.AddDays(-1), "Günlük");
                    }

                    if (now.DayOfWeek == DayOfWeek.Monday && now.Hour == 0 && now.Minute == 10)
                    {
                        var startOfLastWeek = now.Date.AddDays(-7 - (int)now.DayOfWeek);
                        var endOfLastWeek = startOfLastWeek.AddDays(6);
                        await GenerateAndSendReportAsync(startOfLastWeek, endOfLastWeek, "Haftalık");
                    }

                    if (now.Day == 1 && now.Hour == 0 && now.Minute == 20)
                    {
                        var firstDayLastMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
                        var lastDayLastMonth = firstDayLastMonth.AddMonths(1).AddDays(-1);
                        await GenerateAndSendReportAsync(firstDayLastMonth, lastDayLastMonth, "Aylık");
                    }

                    if (now.Month == 1 && now.Day == 1 && now.Hour == 0 && now.Minute == 30)
                    {
                        var lastYear = now.Year - 1;
                        var firstDayLastYear = new DateTime(lastYear, 1, 1);
                        var lastDayLastYear = new DateTime(lastYear, 12, 31);
                        await GenerateAndSendReportAsync(firstDayLastYear, lastDayLastYear, "Yıllık");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ScheduledReportService hata oluştu.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task GenerateAndSendReportAsync(DateTime startDate, DateTime endDate, string reportType)
        {
            _logger.LogInformation($"{reportType} rapor hazırlanıyor: {startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}");

            var users = await _userService.GetAllUsersAsync();
            var barbers = users.Where(u => u.Role == "Barber").ToList();

            foreach (var barber in barbers)
            {
                try
                {
                    var requestDto = new ReportRequestDto
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        BarberId = barber.Id
                    };

                    // Report tipi projenize göre Report veya BerberApp.API.Models.Report olabilir
                    var report = await _reportService.GenerateReportAsync(requestDto);
                    if (report == null)
                    {
                        _logger.LogWarning($"Berber {barber.Email} için {reportType} rapor bulunamadı.");
                        continue;
                    }

                    var completedAppointments = await _appointmentRepository.GetByStatusAndDateRangeAsync(
                        AppointmentStatus.Completed, startDate, endDate, barber.Id);

                    var services = await _serviceRepository.GetAllAsync();

                    var customers = completedAppointments
                        .Select(a => a.User)
                        .Where(u => u != null)
                        .GroupBy(u => u.Id)
                        .Select(g => g.First())
                        .Select(c => new HelperCustomer
                        {
                            Id = c.Id,
                            FullName = c.FullName,
                            Email = c.Email,
                            Phone = c.PhoneNumber ?? ""
                        }).ToList();

                    var servicesList = services.Select(s => new HelperService
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToList();

                    var appointmentsList = completedAppointments.SelectMany(a =>
                        a.AppointmentServices.Select(asrv => new HelperAppointment
                        {
                            Id = a.Id,
                            ServiceId = asrv.ServiceId,
                            Date = a.AppointmentDate
                        })
                    ).ToList();

                    var pdfFileName = $"{barber.Id}_{reportType}_{startDate:yyyyMMdd}.pdf";
                    var pdfFilePath = Path.Combine("Reports", pdfFileName);

                    var pdfGenerator = new BerberApp.API.Helpers.PdfGenerator();
                    pdfGenerator.GenerateReport(pdfFilePath, customers, servicesList, appointmentsList);

                    var subject = $"{reportType} Rapor - {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}";
                    var body = CreateReportEmailBody(report, barber, reportType);

                    // PDF dosyasını byte[] olarak oku
                    var pdfBytes = await File.ReadAllBytesAsync(pdfFilePath);

                    // Mail gönderimi (byte[] ve dosya adı ile)
                    await _emailService.SendEmailWithAttachmentAsync(
                        barber.Email,
                        subject,
                        body,
                        pdfBytes,
                        pdfFileName);

                    _logger.LogInformation($"{barber.Email} adresine {reportType} rapor ve PDF gönderildi.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Berber {barber.Email} için {reportType} rapor gönderiminde hata oluştu.");
                }
            }
        }

        // Burada Report tipini DTO veya Model ne ise ona göre kullan.
        private string CreateReportEmailBody(Report report, UserDto barber, string reportType)
        {
            return $@"
                Merhaba {barber.FullName},<br/>
                <br/>
                {reportType} raporunuz ({report.StartDate:dd.MM.yyyy} - {report.EndDate:dd.MM.yyyy}) aşağıdaki gibidir:<br/>
                <ul>
                    <li>Toplam Randevu: {report.TotalAppointments}</li>
                    <li>Bekleyen Randevu: {report.PendingAppointments}</li>
                    <li>Onaylanan Randevu: {report.ConfirmedAppointments}</li>
                    <li>Tamamlanan Randevu: {report.CompletedAppointments}</li>
                    <li>İptal Edilen Randevu: {report.CancelledAppointments}</li>
                    <li>Toplam Gelir: {report.TotalRevenue:C}</li>
                    <li>Tamamlanan Gelir: {report.CompletedRevenue:C}</li>
                    <li>Onaylanan Gelir: {report.ConfirmedRevenue:C}</li>
                </ul>
                <br/>
                İyi çalışmalar,<br/>
                BerberApp Ekibi
            ";
        }
    }
}
