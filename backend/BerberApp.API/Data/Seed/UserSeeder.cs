using BerberApp.API.Data;
using BerberApp.API.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BerberApp.API.Data
{
    public static class UserSeeder
    {
        /// <summary>
        /// Veritabanına başlangıç kullanıcılarını ekler (eğer yoksa).
        /// </summary>
        public static void Seed(ApplicationDbContext context)
        {
            // Admin kullanıcı var mı?
            var adminEmail = "admin@deneme.com";
            var adminUser = context.Users.FirstOrDefault(u => u.Email == adminEmail);

            if (adminUser == null)
            {
                // Admin kullanıcı oluştur
                CreatePasswordHash("Admin123!", out byte[] passwordHash, out byte[] passwordSalt);
                adminUser = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    FullName = "Admin Deneme",
                    PhoneNumber = "05551234567",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                };
                context.Users.Add(adminUser);
                context.SaveChanges(); // 🔴 BURASI ZORUNLU, yoksa Id oluşmaz
            }

            // Admin rolünü veritabanından çek
            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "admin");
            if (adminRole == null)
            {
                // Admin rolü yoksa oluştur
                adminRole = new Role { Name = "admin" };
                context.Roles.Add(adminRole);
                context.SaveChanges();
            }

            // Admin rolü atanmış mı?
            bool hasAdminRole = context.UserRoles.Any(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);
            if (!hasAdminRole)
            {
                context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                });
                context.SaveChanges();
            }


            // Diğer kullanıcılar varsa onları da ekle
            if (!context.Users.Any(u => u.Email == "user1@example.com"))
                CreateUser(context, "user1@example.com", "John Doe", "user1234", "05559876543", isActive: true);

            if (!context.Users.Any(u => u.Email == "user2@example.com"))
                CreateUser(context, "user2@example.com", "Jane Smith", "user1234", "05551239876", isActive: true);

            context.SaveChanges(); // Son kez tüm kalan işlemleri kaydet
        }

        /// <summary>
        /// Yeni kullanıcı oluşturur ve veritabanına ekler.
        /// </summary>
        private static void CreateUser(ApplicationDbContext context, string email, string fullName, string password, string phoneNumber, bool isActive)
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Email = email,
                UserName = email,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            context.Users.Add(user);
        }

        /// <summary>
        /// Parola için hash ve salt oluşturur.
        /// </summary>
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
