using MediatR;

namespace GirlyShopBackend.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(int Id) : IRequest<bool>;
