namespace ShopMicroservice.Core.Domain
{
    public enum ProductStatus
    {
        Draft = 0,
        Published = 1,
        Archived = 2
    }

    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public long CategoryId { get; set; }
        public long? BrandId { get; set; }

        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }

        public decimal BasePrice { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Draft;
        public bool IsFeatured { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }

    public class ProductImage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; } = default!;
        public int SortOrder { get; set; }
        public bool IsPrimary { get; set; }
    }

    /// <summary>
    /// هر واریانت (مثلاً سایز/رنگ) SKU، قیمت و موجودی مستقل خودش را دارد.
    /// </summary>
    public class ProductVariant
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public string Sku { get; set; } = default!;
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<ProductVariantAttribute> Attributes { get; set; }
            = new List<ProductVariantAttribute>();
    }

    /// <summary>
    /// مقدار یک ویژگی (مثلاً "سایز: L" یا "رنگ: مشکی") روی یک واریانت خاص.
    /// </summary>
    public class ProductVariantAttribute
    {
        public Guid VariantId { get; set; }
        public string AttributeName { get; set; } = default!;   // "سایز", "رنگ"
        public string AttributeValue { get; set; } = default!;  // "L", "مشکی"
    }

    public class Category
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public bool IsActive { get; set; } = true;
    }

    public class Brand
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public bool IsActive { get; set; } = true;
    }
}
