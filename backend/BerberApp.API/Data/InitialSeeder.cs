using BerberApp.API.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BerberApp.API.Data
{
    public static class InitialSeed
    {
        /// <summary>
        /// Örnek kullanıcıları ekler (özellikle admin).
        /// </summary>
        public static void SeedUsers(ApplicationDbContext context)
        {
            if (context.Users.Any()) return; // Zaten kullanıcı varsa atla

            CreatePasswordHash("admin123", out byte[] passwordHash, out byte[] passwordSalt);

            var adminUser = new User
            {
                FullName = "Admin User",
                Email = "admin@berberapp.com",
                UserName = "admin@berberapp.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                PhoneNumber = "05551234567",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            context.Users.Add(adminUser);
            context.SaveChanges();
        }

        /// <summary>
        /// Örnek servisleri ekler.
        /// </summary>
        public static void SeedServices(ApplicationDbContext context)
        {
            if (context.Services.Any()) return;

            var services = new[]
            {
                new Service { Name = "Saç Kesimi", Price = 50m, DurationMinutes = 30, Description = "Standart saç kesimi." },
                new Service { Name = "Sakal Traşı", Price = 30m, DurationMinutes = 20, Description = "Konforlu sakal traşı." },
                new Service { Name = "Saç Boyama", Price = 150m, DurationMinutes = 90, Description = "Profesyonel saç boyama." }
            };

            context.Services.AddRange(services);
            context.SaveChanges();
        }

        /// <summary>
        /// Örnek roller (UserRole) ekler.
        /// </summary>
        public static void SeedRoles(ApplicationDbContext context)
        {
            if (context.Roles.Any() == false)
            {
                var roles = new[]
                {
            new Role { Name = "Admin" },
            new Role { Name = "Berber" },
            new Role { Name = "Müşteri" }
        };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            var adminUser = context.Users.FirstOrDefault(u => u.Email == "admin@berberapp.com");
            if (adminUser == null)
            {
                throw new Exception("Admin user not found. SeedUsers must run before SeedRoles.");
            }

            var adminRole = context.Roles.First(r => r.Name == "Admin");
            var berberRole = context.Roles.First(r => r.Name == "Berber");
            var musteriRole = context.Roles.First(r => r.Name == "Müşteri");

            if (!context.UserRoles.Any(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id))
            {
                context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                });
            }

            if (!context.UserRoles.Any(ur => ur.UserId == adminUser.Id && ur.RoleId == berberRole.Id))
            {
                context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = berberRole.Id
                });
            }

            if (!context.UserRoles.Any(ur => ur.UserId == adminUser.Id && ur.RoleId == musteriRole.Id))
            {
                context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = musteriRole.Id
                });
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Basit password hashing fonksiyonu (bcrypt daha iyi ama örnek için).
        /// </summary>
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
