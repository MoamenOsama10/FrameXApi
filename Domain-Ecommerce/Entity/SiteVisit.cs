using Domain_Ecommerce.Common;

namespace Domain_Ecommerce.Entity
{
    public class SiteVisit : BaseEntity
    {
        // معرّف عشوائي بيتخزن في متصفح الزائر
        public string VisitorId { get; set; } = string.Empty;

        // تاريخ اليوم بتوقيت مصر (من غير وقت)
        public DateTime VisitDate { get; set; }

        public DateTime FirstSeenAt { get; set; } = DateTime.UtcNow;
        public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;
        public int PageViews { get; set; } = 1;

        // بيتملي لو الزائر معروف (مسجّل دخول أو عمل أوردر)
        public string? Email { get; set; }
        public string? UserAgent { get; set; }
    }
}