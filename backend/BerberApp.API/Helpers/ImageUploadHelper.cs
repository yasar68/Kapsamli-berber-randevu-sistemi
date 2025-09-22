using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BerberApp.API.Helpers
{
    public class ImageUploadHelper
    {
        private readonly string _basePath; // Dosyaların kaydedileceği kök klasör

        public ImageUploadHelper(string basePath)
        {
            _basePath = basePath;
        }

        /// <summary>
        /// Gelen IFormFile dosyasını belirtilen klasöre kaydeder.
        /// </summary>
        /// <param name="file">Yüklenecek dosya</param>
        /// <param name="folder">Alt klasör ismi (örneğin "profile", "barbers")</param>
        /// <returns>Kaydedilen dosyanın tam yolu veya null</returns>
        public async Task<string?> UploadImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            // İzin verilen uzantılar (sadece örnek)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || Array.IndexOf(allowedExtensions, ext) < 0)
                return null; // Geçersiz uzantı

            // Kaydedilecek klasörün yolu
            var folderPath = Path.Combine(_basePath, folder);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Benzersiz dosya adı oluştur
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folderPath, fileName);

            // Dosyayı kaydet
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Burada dosya yolu olarak göreceli yol ya da URL dönebilirsin, proje ihtiyacına göre değişir.
            return fileName; 
        }
    }
}
