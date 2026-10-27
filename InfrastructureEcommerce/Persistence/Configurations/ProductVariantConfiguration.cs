using Domain_Ecommerce.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureEcommerce.Persistence.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Color).IsRequired().HasMaxLength(50);
            builder.Property(v => v.Price).HasColumnType("decimal(18,2)");
            builder.Property(v => v.SKU).IsRequired().HasMaxLength(50);
            builder.HasIndex(v => v.SKU).IsUnique();

            builder.HasMany(v => v.Images)
                .WithOne(i => i.ProductVariant)
                .HasForeignKey(i => i.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.CartItems)
                .WithOne(ci => ci.ProductVariant)
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.OrderItems)
                .WithOne(oi => oi.ProductVariant)
                .HasForeignKey(oi => oi.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
