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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
builder.HasQueryFilter(o => !o.IsDeleted);
            builder.Property(o => o.UserId).IsRequired();
            builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(30);
            builder.HasIndex(o => o.OrderNumber).IsUnique();

            builder.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
            builder.Property(o => o.Shipping).HasColumnType("decimal(18,2)");
            builder.Property(o => o.Tax).HasColumnType("decimal(18,2)");
            builder.Property(o => o.Total).HasColumnType("decimal(18,2)");
            builder.Property(o => o.DepositAccount).HasMaxLength(50);
            builder.Property(o => o.DepositReceiptUrl).HasMaxLength(500);
           builder.HasOne(o => o.ShippingAddress)
    .WithMany(a => a.Orders)
    .HasForeignKey(o => o.ShippingAddressId)
    .OnDelete(DeleteBehavior.Restrict)
    .IsRequired(false);

            builder.HasOne(o => o.PaymentMethod)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
                builder.Property(o => o.CustomerName).HasMaxLength(100);
builder.Property(o => o.CustomerEmail).HasMaxLength(200);
builder.Property(o => o.CustomerPhone).HasMaxLength(20);
builder.Property(o => o.City).HasMaxLength(100);
builder.Property(o => o.AddressDetails).HasMaxLength(500);
builder.Property(o => o.Notes).HasMaxLength(1000);

        }
    }
}
