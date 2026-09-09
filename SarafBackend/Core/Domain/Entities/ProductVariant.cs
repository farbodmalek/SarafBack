namespace GirlyShopBackend.Core.Domain.Entities;

public class ProductVariant
{
    public int Id { get; set; }
    public string? Sku { get; set; }
    public int Stock { get; set; }
    public decimal? PriceOverride { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public ICollection<VariantAttributeValue> AttributeValues { get; set; } = new List<VariantAttributeValue>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
