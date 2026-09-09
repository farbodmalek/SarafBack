namespace GirlyShopBackend.Core.Domain.Entities;

public class VariantAttributeValue
{
    public int Id { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;

    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
}
