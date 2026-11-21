using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopMicroservice.Core.Domain;

namespace ShopMicroservice.Infrastructure.Persistence.EntityFramework
{
    public class AddressEntityConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Title).IsRequired().HasMaxLength(150);
            builder.Property(a => a.RecipientName).HasMaxLength(150);
            builder.Property(a => a.PhoneNumber).HasMaxLength(20);
            builder.Property(a => a.City).HasMaxLength(100);
            builder.Property(a => a.PostalCode).HasMaxLength(20);
            builder.Property(a => a.Detail).IsRequired().HasMaxLength(500);
            builder.HasIndex(a => a.UserId);
        }
    }

    public class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");
            builder.HasKey(c => c.Id);
            builder.HasIndex(c => new { c.UserId, c.VariantId }).IsUnique();
        }
    }

    public class WishlistItemEntityConfiguration : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            builder.ToTable("WishlistItems");
            builder.HasKey(w => w.Id);
            builder.HasIndex(w => new { w.UserId, w.ProductId }).IsUnique();
        }
    }

    public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);

            // ستون محاسبه‌ای در دیتابیس: EF آن را نمی‌نویسد، فقط بعد از Insert می‌خواند
            builder.Property(o => o.OrderNumber)
                .HasMaxLength(20)
                .HasComputedColumnSql("(N'AV-' + CAST(100000 + [Id] AS NVARCHAR(12)))", stored: true);

            builder.Property(o => o.RecipientName).IsRequired().HasMaxLength(150);
            builder.Property(o => o.RecipientPhone).IsRequired().HasMaxLength(20);
            builder.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(700);
            builder.Property(o => o.ShippingMethod).IsRequired().HasMaxLength(30);
            builder.Property(o => o.PaymentMethod).IsRequired().HasMaxLength(30);
            builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(o => o.Subtotal).HasColumnType("decimal(14,2)");
            builder.Property(o => o.ShippingCost).HasColumnType("decimal(14,2)");
            builder.Property(o => o.Total).HasColumnType("decimal(14,2)");

            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.UserId);
        }
    }

    public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.ProductName).IsRequired().HasMaxLength(255);
            builder.Property(i => i.VariantDescription).HasMaxLength(300);
            builder.Property(i => i.Sku).HasMaxLength(100);
            builder.Property(i => i.ImageUrl).HasMaxLength(500);
            builder.Property(i => i.UnitPrice).HasColumnType("decimal(14,2)");
        }
    }
}
