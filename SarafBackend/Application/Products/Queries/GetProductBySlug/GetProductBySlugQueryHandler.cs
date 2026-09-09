using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using GirlyShopBackend.Core.Domain.ViewModel.Product;
using MediatR;

namespace GirlyShopBackend.Application.Products.Queries.GetProductBySlug;

public class GetProductBySlugQueryHandler
    : IRequestHandler<GetProductBySlugQuery, ProductDetailViewModel?>
{
    private readonly IProductReadRepository _readRepository;

    public GetProductBySlugQueryHandler(IProductReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<ProductDetailViewModel?> Handle(
        GetProductBySlugQuery request, CancellationToken cancellationToken)
    {
        var product = await _readRepository.GetBySlugAsync(request.Slug);
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
}
