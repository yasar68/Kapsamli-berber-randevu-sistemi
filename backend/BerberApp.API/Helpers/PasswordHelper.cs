using System;
using System.Security.Cryptography;
using System.Text;

namespace BerberApp.API.Helpers
{
    public class PasswordHelper
    {
        // Şifreyi hash ve salt olarak oluşturur
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty or whitespace.");

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key; // Rastgele salt üretir
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // Şifre doğrulama: verilen şifre hash’i ve salt ile uyuşuyor mu kontrol eder
        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (storedHash.Length != 64) return false; // HMACSHA512 hash uzunluğu
            if (storedSalt.Length != 128) return false; // HMACSHA512 key uzunluğu

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }
            return true;
        }
    }
}
