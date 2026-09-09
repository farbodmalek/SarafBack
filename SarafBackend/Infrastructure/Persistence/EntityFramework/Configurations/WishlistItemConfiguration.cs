using GirlyShopBackend.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GirlyShopBackend.Infrastructure.Persistence.EntityFramework.Configurations;

public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> builder)
    {
        builder.HasIndex(w => new { w.UserId, w.ProductId }).IsUnique();

        builder.HasOne(w => w.User)
               .WithMany(u => u.WishlistItems)
               .HasForeignKey(w => w.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.Product)
               .WithMany(p => p.WishlistedBy)
               .HasForeignKey(w => w.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
