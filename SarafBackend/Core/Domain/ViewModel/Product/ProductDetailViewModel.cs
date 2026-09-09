namespace GirlyShopBackend.Core.Domain.ViewModel.Product;

public class ProductDetailViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal? DiscountPrice { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;

    public List<string> ImageUrls { get; set; } = new();
    public List<ProductVariantViewModel> Variants { get; set; } = new();
}
