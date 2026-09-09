namespace GirlyShopBackend.Core.Domain.ViewModel.Product;

public class ProductListItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? MainImageUrl { get; set; }
}
