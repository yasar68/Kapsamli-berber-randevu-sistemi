using BerberApp.API.Data;
using BerberApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BerberApp.API.Data
{
    public static class DbInitializer
    {
        /// <summary>
        /// Veritabanını oluşturur ve başlangıç verilerini ekler.
        /// </summary>
        public static void Initialize(ApplicationDbContext context)
        {
            // Veritabanı yoksa oluştur ve varsa migrasyonları uygula
            context.Database.Migrate();

            // Seed işlemini çalıştır
            InitialSeed.SeedUsers(context);
            InitialSeed.SeedServices(context);
            InitialSeed.SeedRoles(context);

            // İstersen buraya başka seed metotları ekleyebilirsin
        }
    }
}
