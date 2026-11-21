using GirlyShop.AuthService.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GirlyShop.AuthService.Infrastructure.Persistence.EntityFramework;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.PhoneNumber).IsUnique();
        builder.Property(u => u.FullName).HasMaxLength(150).IsRequired();
        builder.Property(u => u.PhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(150);
        builder.Property(u => u.PasswordHash).HasMaxLength(300).IsRequired();
    }
}
