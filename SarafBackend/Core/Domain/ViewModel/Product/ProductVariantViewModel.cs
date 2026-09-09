namespace GirlyShopBackend.Core.Domain.ViewModel.Product;

public class ProductVariantAttributeViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class ProductVariantViewModel
{
    public int Id { get; set; }
    public int Stock { get; set; }
    public decimal? PriceOverride { get; set; }
    public List<ProductVariantAttributeViewModel> Attributes { get; set; } = new();
}
