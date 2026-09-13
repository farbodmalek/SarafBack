using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopMicroservice.Core.Domain;

namespace ShopMicroservice.Infrastructure.Persistence.EntityFramework
{
    public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
            builder.Property(p => p.Slug).IsRequired().HasMaxLength(280);
            builder.HasIndex(p => p.Slug).IsUnique();
            builder.Property(p => p.Description).HasMaxLength(4000);
            builder.Property(p => p.BasePrice).HasColumnType("decimal(14,2)");
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

            builder.HasMany(p => p.Images)
                .WithOne()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Variants)
                .WithOne()
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.CategoryId);
            builder.HasIndex(p => p.Status);
        }
    }

    public class ProductImageEntityConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("ProductImages");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.ImageUrl).IsRequired().HasMaxLength(500);
        }
    }

    public class ProductVariantEntityConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariants");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Sku).IsRequired().HasMaxLength(100);
            builder.HasIndex(v => v.Sku).IsUnique();
            builder.Property(v => v.Price).HasColumnType("decimal(14,2)");
            builder.Property(v => v.CompareAtPrice).HasColumnType("decimal(14,2)");

            builder.HasMany(v => v.Attributes)
                .WithOne()
                .HasForeignKey(a => a.VariantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ProductVariantAttributeEntityConfiguration : IEntityTypeConfiguration<ProductVariantAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductVariantAttribute> builder)
        {
            builder.ToTable("ProductVariantAttributes");
            builder.HasKey(a => new { a.VariantId, a.AttributeName });
            builder.Property(a => a.AttributeName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.AttributeValue).IsRequired().HasMaxLength(150);
        }
    }

    public class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
            builder.Property(c => c.Slug).IsRequired().HasMaxLength(170);
            builder.HasIndex(c => c.Slug).IsUnique();
        }
    }

    public class BrandEntityConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brands");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Name).IsRequired().HasMaxLength(150);
            builder.Property(b => b.Slug).IsRequired().HasMaxLength(170);
            builder.HasIndex(b => b.Slug).IsUnique();
        }
    }
}
