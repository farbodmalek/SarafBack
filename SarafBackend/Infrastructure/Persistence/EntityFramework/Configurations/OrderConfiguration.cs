using GirlyShopBackend.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GirlyShopBackend.Infrastructure.Persistence.EntityFramework.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.Property(o => o.ShippingCost).HasColumnType("decimal(18,2)");
        builder.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
        builder.Property(o => o.Total).HasColumnType("decimal(18,2)");
        builder.Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)");
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(o => o.User)
               .WithMany(u => u.Orders)
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.Address)
               .WithMany(a => a.Orders)
               .HasForeignKey(o => o.AddressId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.Coupon)
               .WithMany(c => c.Orders)
               .HasForeignKey(o => o.CouponId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
