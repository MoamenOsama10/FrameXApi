using Domain_Ecommerce.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureEcommerce.Persistence.Configurations
{
    public class SiteVisitConfiguration : IEntityTypeConfiguration<SiteVisit>
    {
        public void Configure(EntityTypeBuilder<SiteVisit> builder)
        {
            builder.HasKey(v => v.Id);
            builder.Property(v => v.VisitorId).IsRequired().HasMaxLength(64);
            builder.Property(v => v.VisitDate).HasColumnType("date");
            builder.Property(v => v.Email).HasMaxLength(200);
            builder.Property(v => v.UserAgent).HasMaxLength(300);

            // زائر واحد = صف واحد في اليوم
            builder.HasIndex(v => new { v.VisitorId, v.VisitDate }).IsUnique();
        }
    }
}