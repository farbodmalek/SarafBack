namespace ShopMicroservice.Application.DTO
{
    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = default!;
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public int StockQuantity { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new();
    }

    public class ProductListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public decimal BasePrice { get; set; }
        public string? PrimaryImageUrl { get; set; }
        public string CategoryName { get; set; } = default!;
        public string? BrandName { get; set; }
        public bool IsFeatured { get; set; }
    }

    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public string Status { get; set; } = default!;
        public long CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public long? BrandId { get; set; }
        public string? BrandName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<ProductVariantDto> Variants { get; set; } = new();
    }

    /// <summary>فیلترهای مشترک برای همه کوئری‌های لیست محصول.</summary>
    public class ProductFilterDto
    {
        public string? SearchTerm { get; set; }
        public long? CategoryId { get; set; }
        public long? BrandId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStockOnly { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
