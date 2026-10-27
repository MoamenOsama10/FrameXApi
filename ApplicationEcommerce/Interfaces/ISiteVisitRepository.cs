using Domain_Ecommerce.Entity;

namespace ApplicationEcommerce.Interfaces
{
    public interface ISiteVisitRepository
    {
        Task<SiteVisit?> GetByVisitorAndDateAsync(string visitorId, DateTime date);
        Task AddAsync(SiteVisit visit);
        void Update(SiteVisit visit);
        Task<IReadOnlyList<SiteVisit>> GetVisitsByDateAsync(DateTime date);
    }
}