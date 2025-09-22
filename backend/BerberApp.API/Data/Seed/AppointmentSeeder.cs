using BerberApp.API.Data;
using BerberApp.API.Models;
using System;
using System.Linq;

namespace BerberApp.API.Data
{
    public static class AppointmentSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Appointments.Any())
                return; // Zaten veri varsa ekleme yapma

            // Örnek randevular ekle
            context.Appointments.AddRange(
                new Appointment
                {
                    UserId = 2, // Örnek müşteri ID'si, veritabanında mevcut olmalı
                    AppointmentDate = DateTime.UtcNow.AddDays(1).AddHours(10), // Yarın saat 10:00
                    DurationMinutes = 60,
                    Price = 150m,
                    Status = AppointmentStatus.Pending,
                    Notes = "Saç kesimi için randevu",
                    CreatedAt = DateTime.UtcNow
                },
                new Appointment
                {
                    UserId = 3,
                    AppointmentDate = DateTime.UtcNow.AddDays(2).AddHours(14), // 2 gün sonra saat 14:00
                    DurationMinutes = 30,
                    Price = 80m,
                    Status = AppointmentStatus.Confirmed,
                    Notes = "Sakal traşı",
                    CreatedAt = DateTime.UtcNow
                },
                new Appointment
                {
                    UserId = 2,
                    AppointmentDate = DateTime.UtcNow.AddDays(-1).AddHours(15), // Dün saat 15:00 (geçmişte)
                    DurationMinutes = 45,
                    Price = 100m,
                    Status = AppointmentStatus.Completed,
                    Notes = "Saç boyama",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            );

            context.SaveChanges();
        }
    }
}
