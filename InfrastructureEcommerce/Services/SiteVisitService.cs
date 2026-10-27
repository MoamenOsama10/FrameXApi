using System.Net.Mail;
using ApplicationEcommerce.Common.Exceptions;
using ApplicationEcommerce.DTOs;
using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfrastructureEcommerce.Services
{
    public class SiteVisitService : ISiteVisitService
    {
        private readonly ISiteVisitRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SiteVisitService> _logger;

        public SiteVisitService(
            ISiteVisitRepository repository,
            IUnitOfWork unitOfWork,
            ILogger<SiteVisitService> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // توقيت مصر: على ويندوز الاسم "Egypt Standard Time" وعلى لينكس "Africa/Cairo"
        private static readonly TimeZoneInfo CairoZone = ResolveCairoZone();

        private static TimeZoneInfo ResolveCairoZone()
        {
            foreach (var id in new[] { "Egypt Standard Time", "Africa/Cairo" })
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
                catch (TimeZoneNotFoundException) { }
            }
            return TimeZoneInfo.Utc;
        }

        private static DateTime TodayInCairo() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CairoZone).Date;

        private static bool IsValidVisitorId(string? id) =>
            !string.IsNullOrWhiteSpace(id)
            && id.Length <= 64
            && id.All(c => char.IsLetterOrDigit(c) || c == '-');

        public async Task RecordVisitAsync(string visitorId, string? userAgent)
        {
            if (!IsValidVisitorId(visitorId))
                throw new BadRequestException("معرّف الزائر غير صالح.");

            var today = TodayInCairo();
            var visit = await _repository.GetByVisitorAndDateAsync(visitorId, today);

            if (visit is not null)
            {
                visit.PageViews++;
                visit.LastSeenAt = DateTime.UtcNow;
                _repository.Update(visit);
                await _unitOfWork.SaveChangesAsync();
                return;
            }

            var newVisit = new SiteVisit
            {
                VisitorId = visitorId,
                VisitDate = today,
                FirstSeenAt = DateTime.UtcNow,
                LastSeenAt = DateTime.UtcNow,
                PageViews = 1,
                UserAgent = userAgent is { Length: > 300 } ? userAgent[..300] : userAgent
            };

            try
            {
                await _repository.AddAsync(newVisit);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // طلبين متزامنين لنفس الزائر: التاني اتسجّل، وخسارة page view واحد مش مهمة
                _logger.LogWarning(ex, "Duplicate visit insert ignored for visitor {VisitorId}", visitorId);
            }
        }

        public async Task AttachEmailAsync(string visitorId, string email)
        {
            if (!IsValidVisitorId(visitorId) || string.IsNullOrWhiteSpace(email))
                return;

            var cleanEmail = email.Trim().ToLowerInvariant();
            if (cleanEmail.Length > 200 || !MailAddress.TryCreate(cleanEmail, out _))
                return;

            var visit = await _repository.GetByVisitorAndDateAsync(visitorId, TodayInCairo());
            if (visit is null)
                return;

            visit.Email = cleanEmail;
            _repository.Update(visit);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<VisitStatsDto> GetTodayStatsAsync()
        {
            var today = TodayInCairo();
            var visits = await _repository.GetVisitsByDateAsync(today);

            return new VisitStatsDto
            {
                Date = today,
                UniqueVisitors = visits.Count,
                TotalPageViews = visits.Sum(v => v.PageViews),
                KnownEmails = visits
                    .Where(v => v.Email != null)
                    .Select(v => v.Email!)
                    .Distinct()
                    .Count(),
                Visits = visits
                    .OrderByDescending(v => v.LastSeenAt)
                    .Select(v => new SiteVisitDto
                    {
                        Email = v.Email,
                        FirstSeenAt = v.FirstSeenAt,
                        LastSeenAt = v.LastSeenAt,
                        PageViews = v.PageViews
                    })
                    .ToList()
            };
        }
    }
}