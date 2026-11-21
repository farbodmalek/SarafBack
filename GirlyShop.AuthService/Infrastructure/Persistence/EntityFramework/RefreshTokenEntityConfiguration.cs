using GirlyShop.AuthService.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GirlyShop.AuthService.Infrastructure.Persistence.EntityFramework;

public class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(t => t.Id);

        // ستون دیتابیس اسمش Token است، ولی داخلش هش ذخیره می‌شود
        builder.Property(t => t.TokenHash).HasColumnName("Token").HasMaxLength(500).IsRequired();
        builder.HasIndex(t => t.TokenHash).IsUnique();
        builder.Property(t => t.CreatedByIp).HasMaxLength(50);

        // IsActive فقط در حافظه محاسبه می‌شود، ستون دیتابیس نیست
        builder.Ignore(t => t.IsActive);
    }
}
