namespace ApplicationEcommerce.DTOs
{
    public class SiteVisitDto
    {
        public string? Email { get; set; }
        public DateTime FirstSeenAt { get; set; }
        public DateTime LastSeenAt { get; set; }
        public int PageViews { get; set; }
    }

    public class VisitStatsDto
    {
        public DateTime Date { get; set; }
        public int UniqueVisitors { get; set; }
        public int TotalPageViews { get; set; }
        public int KnownEmails { get; set; }
        public List<SiteVisitDto> Visits { get; set; } = new();
    }
}