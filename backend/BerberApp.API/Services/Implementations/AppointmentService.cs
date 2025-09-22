using AutoMapper;
using BerberApp.API.DTOs.Appointment;
using BerberApp.API.Models;
using BerberApp.API.Repositories.Interfaces;
using BerberApp.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BerberApp.API.Services.Implementations
{
    // Entity için isim çakışmasını önlemek adına alias kullandım
    using AppointmentServiceEntity = BerberApp.API.Models.AppointmentService;

    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBarberRepository _barberRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IAppointmentServiceRepository _appointmentServiceRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IWorkingHourRepository _workingHourRepository;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IUserRepository userRepository,
            IBarberRepository barberRepository,
            IServiceRepository serviceRepository,
            IAppointmentServiceRepository appointmentServiceRepository,
            IMapper mapper,
            ILogger<AppointmentService> logger,
            IEmailService emailService,
            IWorkingHourRepository workingHourRepository)
        {
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
            _barberRepository = barberRepository;
            _serviceRepository = serviceRepository;
            _appointmentServiceRepository = appointmentServiceRepository;
            _mapper = mapper;
            _emailService = emailService;
            _workingHourRepository = workingHourRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<AppointmentViewDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<AppointmentViewDto>>(appointments);
        }

        public async Task<AppointmentViewDto?> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                return null;

            return _mapper.Map<AppointmentViewDto>(appointment);
        }

        public async Task<IEnumerable<AppointmentViewDto>> GetAppointmentsByUserIdAsync(int userId)
        {
            var appointments = await _appointmentRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<AppointmentViewDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentViewDto>> GetAppointmentsByBarberIdAsync(int barberId)
        {
            var appointments = await _appointmentRepository.GetByBarberIdAsync(barberId);
            return _mapper.Map<IEnumerable<AppointmentViewDto>>(appointments);
        }

        public async Task<AppointmentViewDto> CreateAppointmentAsync(CreateAppointmentDto dto)
        {
            // Servisleri çoklu id ile çekiyoruz (IServiceRepository'de GetByIdsAsync metodunun olması gerekir)
            var services = await _serviceRepository.GetByIdsAsync(dto.ServiceIds);

            if (services == null || !services.Any())
                throw new KeyNotFoundException("Seçilen servisler bulunamadı.");

            // Toplam süre ve fiyatı hesapla
            var totalDuration = services.Sum(s => s.DurationMinutes);
            var totalPrice = services.Sum(s => s.Price);

            // Başlangıç zamanını UTC'ye çevir (local ise)
            var appointmentStartUtc = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Local).ToUniversalTime();
            var appointmentEndUtc = appointmentStartUtc.AddMinutes(totalDuration);

            // Çalışma saatlerini kontrol et
            var workingHours = await _workingHourRepository.GetByBarberIdAsync(dto.BarberId);
            var appointmentDay = dto.StartTime.DayOfWeek;
            var appointmentTime = dto.StartTime.TimeOfDay;

            bool isWithinWorkingHours = workingHours.Any(wh =>
                wh.DayOfWeek == appointmentDay &&
                appointmentTime >= wh.StartTime &&
                appointmentTime < wh.EndTime
            );

            if (!isWithinWorkingHours)
                throw new InvalidOperationException("Randevu, berberin çalışma saatleri dışında alınamaz.");

            if (appointmentStartUtc >= appointmentEndUtc)
                throw new ArgumentException("Bitiş saati başlangıçtan önce olamaz.");

            if (appointmentStartUtc < DateTime.UtcNow.AddMinutes(15))
                throw new ArgumentException("Randevular en az 15 dakika sonrası için oluşturulabilir.");

            // Kullanıcı ve berber var mı kontrol et
            var userExists = await _userRepository.ExistsAsync(dto.CustomerId);
            var barberExists = await _barberRepository.ExistsAsync(dto.BarberId);

            if (!userExists)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            if (!barberExists)
                throw new KeyNotFoundException("Berber bulunamadı.");

            // Yeni randevu oluştur
            var appointment = new Appointment
            {
                AppointmentDate = appointmentStartUtc,
                DurationMinutes = totalDuration,
                UserId = dto.CustomerId,
                BarberId = dto.BarberId,
                Notes = dto.Note,
                Status = AppointmentStatus.Confirmed,
                CreatedAt = DateTime.UtcNow,
                Price = totalPrice
            };

            await _appointmentRepository.AddAsync(appointment);

            // Servisleri randevuya bağla (join entity olarak)
            foreach (var service in services)
            {
                var appointmentServiceEntity = new AppointmentServiceEntity
                {
                    AppointmentId = appointment.Id,
                    ServiceId = service.Id,
                    Price = service.Price
                };
                await _appointmentServiceRepository.AddAsync(appointmentServiceEntity);
            }

            var fullAppointment = await _appointmentRepository.GetByIdAsync(appointment.Id);
            if (fullAppointment == null)
                throw new Exception("Randevu oluşturuldu ancak veritabanından çekilemedi.");

            // Mail gönderimi - hata durumunda logla
            try
            {
                var user = await _userRepository.GetByIdAsync(dto.CustomerId);
                var barber = await _barberRepository.GetByIdAsync(dto.BarberId);

                if (!string.IsNullOrEmpty(user?.Email) && dto.EmailBildirimiGonder)
                {
                    var userSubject = "Randevunuz başarıyla oluşturuldu!";
                    var userBody = $"""
                    Merhaba {user.FullName},<br/>
                    {appointment.AppointmentDate.ToLocalTime():dd.MM.yyyy HH:mm} tarihinde randevunuz başarıyla oluşturuldu.<br/>
                    Berber: {barber?.FullName}<br/>
                    Berber Telefon: {barber?.Phone ?? "Belirtilmemiş"}<br/>
                    Hizmetler: {string.Join(", ", services.Select(s => s.Name))}<br/><br/>
                    BerberApp
                    """;

                    await _emailService.SendEmailAsync(user.Email, userSubject, userBody);
                }

                if (!string.IsNullOrEmpty(barber?.Email))
                {
                    var barberSubject = "Yeni bir randevu aldınız!";
                    var barberBody = $"""
                    Merhaba {barber.FullName},<br/>
                    {appointment.AppointmentDate.ToLocalTime():dd.MM.yyyy HH:mm} tarihinde yeni bir randevu aldınız.<br/>
                    Müşteri: {user?.FullName} - {user?.Email}<br/>
                    Müşteri Telefon: {user?.PhoneNumber ?? "Belirtilmemiş"}<br/>
                    Hizmetler: {string.Join(", ", services.Select(s => s.Name))}<br/><br/>
                    BerberApp
                    """;

                    await _emailService.SendEmailAsync(barber.Email, barberSubject, barberBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu oluşturulurken mail gönderiminde hata oluştu.");
            }

            return _mapper.Map<AppointmentViewDto>(fullAppointment);
        }

        public async Task<IEnumerable<(TimeSpan Start, TimeSpan End)>> GetAvailableTimeSlotsAsync(int barberId, DateTime date)
        {
            var workingHours = await _workingHourRepository.GetByBarberIdAsync(barberId);
            var dayWorkingHours = workingHours.Where(wh => wh.DayOfWeek == date.DayOfWeek).ToList();

            if (!dayWorkingHours.Any())
                return Enumerable.Empty<(TimeSpan, TimeSpan)>();

            var appointments = await _appointmentRepository.GetAppointmentsByBarberAndDateAsync(barberId, date.Date);

            var bookedSlots = appointments.Select(a =>
                (
                    Start: a.AppointmentDate.ToLocalTime().TimeOfDay,
                    End: a.AppointmentDate.ToLocalTime().TimeOfDay.Add(TimeSpan.FromMinutes(a.DurationMinutes))
                )
            ).OrderBy(slot => slot.Start).ToList();

            var availableSlots = new List<(TimeSpan Start, TimeSpan End)>();

            foreach (var wh in dayWorkingHours)
            {
                TimeSpan currentStart = wh.StartTime;

                foreach (var booked in bookedSlots)
                {
                    if (booked.Start >= wh.EndTime)
                        break;

                    if (booked.Start > currentStart)
                        availableSlots.Add((currentStart, booked.Start));

                    if (booked.End > currentStart)
                        currentStart = booked.End;
                }

                if (currentStart < wh.EndTime)
                    availableSlots.Add((currentStart, wh.EndTime));
            }

            return availableSlots;
        }

        public async Task<AppointmentViewDto?> UpdateAppointmentAsync(UpdateAppointmentDto dto)
        {
            var appointmentStartUtc = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Local).ToUniversalTime();
            var appointmentEndUtc = DateTime.SpecifyKind(dto.EndTime, DateTimeKind.Local).ToUniversalTime();

            var userExists = await _userRepository.ExistsAsync(dto.CustomerId);
            var barberExists = await _barberRepository.ExistsAsync(dto.BarberId);
            var serviceExists = await _serviceRepository.ExistsAsync(dto.ServiceId);

            if (!userExists || !barberExists || !serviceExists)
                throw new KeyNotFoundException("Kullanıcı, berber veya servis bulunamadı.");

            var appointment = await _appointmentRepository.GetByIdAsync(dto.Id);
            if (appointment == null)
                return null;

            var originalNotes = appointment.Notes;
            var originalStatus = appointment.Status;

            appointment.UserId = dto.CustomerId;
            appointment.BarberId = dto.BarberId;
            // Not: Eğer Appointment entity'sinde ServiceId yoksa bu satırı kaldır veya servis ilişkisini ayrı yönet
            // appointment.ServiceId = dto.ServiceId;

            appointment.AppointmentDate = appointmentStartUtc;
            appointment.DurationMinutes = (int)(appointmentEndUtc - appointmentStartUtc).TotalMinutes;

            appointment.Notes = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note;
            appointment.Status = dto.Status;

            var service = await _serviceRepository.GetByIdAsync(dto.ServiceId);
            appointment.Price = service?.Price ?? appointment.Price;

            if (originalNotes != appointment.Notes || originalStatus != appointment.Status)
            {
                appointment.UpdatedAt = DateTime.UtcNow;
                _logger.LogInformation($"Appointment {dto.Id} notes or status updated");
            }

            await _appointmentRepository.UpdateAsync(appointment);

            return _mapper.Map<AppointmentViewDto>(appointment);
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning($"Silinecek randevu bulunamadı: {id}");
                return false;
            }

            try
            {
                await _appointmentRepository.DeleteAsync(appointment);
                _logger.LogInformation($"Randevu silindi: {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Randevu silinemedi: {id}");
                throw;
            }
        }

        public async Task<IEnumerable<AppointmentViewDto>> GetAppointmentsAsync(int? customerId, int? barberId, string? status)
        {
            AppointmentStatus? statusEnum = status?.ToLower() switch
            {
                "all" or null => null,
                "pending" => AppointmentStatus.Pending,
                "confirmed" => AppointmentStatus.Confirmed,
                "cancelled" => AppointmentStatus.Cancelled,
                "completed" => AppointmentStatus.Completed,
                _ => null
            };

            var appointments = await _appointmentRepository.GetAppointmentsAsync(customerId, barberId, statusEnum);
            return _mapper.Map<IEnumerable<AppointmentViewDto>>(appointments);
        }
        public async Task<IEnumerable<AppointmentViewDto>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int? barberId = null)
        {
            // Repository'de buna uygun bir methodun olduğunu varsayıyorum
            var appointments = await _appointmentRepository.GetByDateRangeAsync(startDate, endDate, barberId);

            // Model -> DTO dönüşümü
            return appointments.Select(a => new AppointmentViewDto
            {
                Id = a.Id,
                StartTime = a.AppointmentDate.ToLocalTime(), // StartTime yok, AppointmentDate var
                EndTime = a.AppointmentDate.AddMinutes(a.DurationMinutes).ToLocalTime(), // EndTime yok, hesapla
                ServiceName = a.AppointmentServices.FirstOrDefault()?.Service?.Name ?? "", // İlk servis adı örnek
                BarberName = a.Barber?.FullName ?? "", // Barber.Name değil FullName olabilir
                CustomerName = a.User?.FullName ?? "Misafir", // Customer yerine User
                Status = a.Status.ToString(), // enum -> string çevir
                ServiceNames = (a.AppointmentServices != null)
                ? a.AppointmentServices
                .Where(asvc => asvc.Service != null)
                .Select(asvc => asvc.Service!.Name)
                .ToList()
                : new List<string>(),
                Price = a.Price,
                Notes = a.Notes
            });
        }
    }
}
