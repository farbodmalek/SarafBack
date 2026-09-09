using GirlyShopBackend.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GirlyShopBackend.Infrastructure.Persistence.EntityFramework.Configurations;

public class VariantAttributeValueConfiguration : IEntityTypeConfiguration<VariantAttributeValue>
{
    public void Configure(EntityTypeBuilder<VariantAttributeValue> builder)
    {
        builder.Property(a => a.AttributeName).HasMaxLength(50);
        builder.Property(a => a.AttributeValue).HasMaxLength(100);

        builder.HasOne(a => a.ProductVariant)
               .WithMany(v => v.AttributeValues)
               .HasForeignKey(a => a.ProductVariantId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
