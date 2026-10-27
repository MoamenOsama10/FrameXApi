using ApplicationEcommerce.Common.Exceptions;

namespace ApplicationEcommerce.Common
{
    public static class ImageUploadValidator
    {
        public const long MaxBytes = 10 * 1024 * 1024; // 10 ميجا

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        // الـ stream لازم يكون قابل للرجوع للأول (IFormFile.OpenReadStream بيكون كده)
        public static void Validate(Stream stream, string fileName, long length)
        {
            if (length <= 0)
                throw new BadRequestException("الملف فاضي.");

            if (length > MaxBytes)
                throw new BadRequestException("حجم الصورة أكبر من 10 ميجا.");

            var extension = Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new BadRequestException("نوع الملف غير مدعوم. المسموح: JPG و PNG و WebP.");

            // نقرا أول بايتات الملف ونتأكد إنها بتاعة صورة فعلاً، مش بس الامتداد
            var header = new byte[12];
            var read = stream.Read(header, 0, header.Length);
            stream.Position = 0;

            if (!LooksLikeImage(header, read))
                throw new BadRequestException("الملف مش صورة صالحة.");
        }

        private static bool LooksLikeImage(byte[] h, int read)
        {
            if (read < 12) return false;

            var jpeg = h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF;
            var png = h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47;
            var webp = h[0] == 0x52 && h[1] == 0x49 && h[2] == 0x46 && h[3] == 0x46
                    && h[8] == 0x57 && h[9] == 0x45 && h[10] == 0x42 && h[11] == 0x50;

            return jpeg || png || webp;
        }
    }
}