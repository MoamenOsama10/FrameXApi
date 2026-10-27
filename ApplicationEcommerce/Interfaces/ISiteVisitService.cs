using ApplicationEcommerce.DTOs;

namespace ApplicationEcommerce.Interfaces
{
    public interface ISiteVisitService
    {
        // بيتنادى من endpoint عام، من غير إيميل
        Task RecordVisitAsync(string visitorId, string? userAgent);

        // بيتنادى من جوه الـ Backend بس (أوردر، رسالة تواصل)
        Task AttachEmailAsync(string visitorId, string email);

        Task<VisitStatsDto> GetTodayStatsAsync();
    }
}