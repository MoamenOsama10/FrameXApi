using ApplicationEcommerce.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureEcommerce.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder)
        {
            // اسم الفولدر: نقبل بس حروف وأرقام و - و _ في كل جزء، وأي "." أو ".." بتتشال
            var segments = (folder ?? string.Empty)
                .Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new string(s.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').ToArray()))
                .Where(s => s.Length > 0)
                .ToArray();

            if (segments.Length == 0)
                throw new ArgumentException("Invalid folder.", nameof(folder));

            // الامتداد بس هو اللي بنحتفظ بيه من اسم الملف، ومنضّف من أي رموز غريبة
            var rawExtension = Path.GetExtension(Path.GetFileName(fileName ?? string.Empty));
            var cleanExtension = new string(rawExtension.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
            if (cleanExtension.Length > 5) cleanExtension = cleanExtension[..5];
            var extension = cleanExtension.Length > 0 ? "." + cleanExtension : string.Empty;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", Path.Combine(segments));

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // الاسم الأصلي مبيدخلش المسار خالص
            var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var output = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(output);
            }

            return $"/uploads/{string.Join("/", segments)}/{uniqueFileName}";
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            var root = Path.GetFullPath(_environment.WebRootPath);
            var fullPath = Path.GetFullPath(Path.Combine(root, filePath.TrimStart('/', '\\')));

            // مسموح نمسح جوه wwwroot بس
            if (!fullPath.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                return;

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}