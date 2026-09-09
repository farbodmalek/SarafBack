using GirlyShopBackend.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GirlyShopBackend.Infrastructure.Persistence.EntityFramework.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.PhoneNumber).IsUnique();
        builder.Property(u => u.FullName).HasMaxLength(150);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.Property(u => u.Email).HasMaxLength(150);
        builder.Property(u => u.PasswordHash).HasMaxLength(300);
    }
}
