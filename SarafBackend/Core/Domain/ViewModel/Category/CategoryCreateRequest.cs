namespace GirlyShopBackend.Core.Domain.ViewModel.Category;

// ورودی فرم ساخت/ویرایش دسته‌بندی
public class CategoryCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public string? ImageUrl { get; set; }
}
