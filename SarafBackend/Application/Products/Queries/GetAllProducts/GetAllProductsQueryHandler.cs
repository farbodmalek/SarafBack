using GirlyShopBackend.Core.Domain.Entities;
using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using GirlyShopBackend.Core.Domain.ViewModel.Product;
using MediatR;

namespace GirlyShopBackend.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler
    : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductListItemViewModel>>
{
    private readonly IProductReadRepository _readRepository;

    public GetAllProductsQueryHandler(IProductReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IEnumerable<ProductListItemViewModel>> Handle(
        GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = string.IsNullOrEmpty(request.CategorySlug)
            ? await _readRepository.GetAllAsync()
            : await _readRepository.GetByCategorySlugAsync(request.CategorySlug);

        return products.Select(MapToListItem);
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
}
