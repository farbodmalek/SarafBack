using MediatR;

namespace GirlyShopBackend.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    int Id,
    string Title,
    string Description,
    decimal BasePrice,
    decimal? DiscountPrice,
    int CategoryId
) : IRequest<bool>;
