namespace GirlyShopBackend.Core.Domain.ViewModel.Product;

public class ProductVariantInput
{
    public string? Sku { get; set; }
    public int Stock { get; set; }
    public decimal? PriceOverride { get; set; }
    public List<ProductVariantAttributeViewModel> Attributes { get; set; } = new();
}
