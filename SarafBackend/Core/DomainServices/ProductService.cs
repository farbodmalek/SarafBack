using GirlyShopBackend.Core.Domain.Entities;
using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using GirlyShopBackend.Core.Domain.ViewModel.Product;
using GirlyShopBackend.Core.DomainServices.Interfaces;

namespace GirlyShopBackend.Core.DomainServices;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductListItemViewModel>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToListItem);
    }

    public async Task<IEnumerable<ProductListItemViewModel>> GetByCategorySlugAsync(string categorySlug)
    {
        var products = await _productRepository.GetByCategorySlugAsync(categorySlug);
        return products.Select(MapToListItem);
    }

    public async Task<ProductDetailViewModel?> GetBySlugAsync(string slug)
    {
        var product = await _productRepository.GetBySlugAsync(slug);
        if (product is null) return null;

        return new ProductDetailViewModel
        {
            Id = product.Id,
            Title = product.Title,
            Slug = product.Slug,
            Description = product.Description,
            BasePrice = product.BasePrice,
            DiscountPrice = product.DiscountPrice,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            CategorySlug = product.Category.Slug,
            ImageUrls = product.Images.OrderBy(i => i.SortOrder).Select(i => i.Url).ToList(),
            Variants = product.Variants.Select(v => new ProductVariantViewModel
            {
                Id = v.Id,
                Stock = v.Stock,
                PriceOverride = v.PriceOverride,
                Attributes = v.AttributeValues.Select(a => new ProductVariantAttributeViewModel
                {
                    Name = a.AttributeName,
                    Value = a.AttributeValue
                }).ToList()
            }).ToList()
        };
    }

    public async Task<int> CreateAsync(ProductCreateRequest request)
    {
        var slug = GenerateSlug(request.Title);

        // اگه اسلاگ تکراری بود، یه پسوند عددی اضافه کن
        var baseSlug = slug;
        var counter = 1;
        while (await _productRepository.SlugExistsAsync(slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        var product = new Product
        {
            Title = request.Title,
            Slug = slug,
            Description = request.Description,
            BasePrice = request.BasePrice,
            DiscountPrice = request.DiscountPrice,
            CategoryId = request.CategoryId,
            Variants = request.Variants.Select(v => new ProductVariant
            {
                Sku = v.Sku,
                Stock = v.Stock,
                PriceOverride = v.PriceOverride,
                AttributeValues = v.Attributes.Select(a => new VariantAttributeValue
                {
                    AttributeName = a.Name,
                    AttributeValue = a.Value
                }).ToList()
            }).ToList()
        };

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return product.Id;
    }

    public async Task<bool> UpdateAsync(int id, ProductCreateRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null) return false;

        product.Title = request.Title;
        product.Description = request.Description;
        product.BasePrice = request.BasePrice;
        product.DiscountPrice = request.DiscountPrice;
        product.CategoryId = request.CategoryId;

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null) return false;

        _productRepository.Remove(product);
        await _productRepository.SaveChangesAsync();
        return true;
    }

    private static ProductListItemViewModel MapToListItem(Product p) => new()
    {
        Id = p.Id,
        Title = p.Title,
        Slug = p.Slug,
        CategoryName = p.Category?.Name ?? string.Empty,
        BasePrice = p.BasePrice,
        DiscountPrice = p.DiscountPrice,
        MainImageUrl = p.Images?.FirstOrDefault(i => i.IsMain)?.Url ?? p.Images?.FirstOrDefault()?.Url
    };

    private static string GenerateSlug(string title)
    {
        // ساده‌سازی فارسی به لاتین برای URL نیاز به کتابخانه جدا داره؛
        // فعلاً فاصله‌ها رو با خط‌تیره جایگزین می‌کنیم و کاراکترهای غیرمجاز رو حذف می‌کنیم
        var slug = title.Trim().Replace(" ", "-");
        return slug;
    }
}
