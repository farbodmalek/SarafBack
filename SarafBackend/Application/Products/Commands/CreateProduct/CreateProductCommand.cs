using GirlyShopBackend.Core.Domain.ViewModel.Product;
using MediatR;

namespace GirlyShopBackend.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Title,
    string Description,
    decimal BasePrice,
    decimal? DiscountPrice,
    int CategoryId,
    List<ProductVariantInput> Variants
) : IRequest<int>;
