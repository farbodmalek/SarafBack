namespace GirlyShopBackend.Core.Domain.ViewModel.Product;

// ورودی فرم «افزودن محصول» در پنل ادمین
public class ProductVariantCreateRequest
{
    public string? Sku { get; set; }
    public int Stock { get; set; }
    public decimal? PriceOverride { get; set; }
    public List<ProductVariantAttributeViewModel> Attributes { get; set; } = new();
}

public class ProductCreateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int CategoryId { get; set; }
    public List<ProductVariantCreateRequest> Variants { get; set; } = new();

    // تصاویر جداگانه به‌صورت multipart/form-data ارسال و در Controller پردازش می‌شوند
}
