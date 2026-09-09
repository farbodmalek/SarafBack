using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using MediatR;

namespace GirlyShopBackend.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductWriteRepository _writeRepository;

    public UpdateProductCommandHandler(IProductWriteRepository writeRepository)
    {
        _writeRepository = writeRepository;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _writeRepository.GetByIdForUpdateAsync(request.Id);
        if (product is null) return false;

        product.Title = request.Title;
        product.Description = request.Description;
        product.BasePrice = request.BasePrice;
        product.DiscountPrice = request.DiscountPrice;
        product.CategoryId = request.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;

        _writeRepository.Update(product);
        await _writeRepository.SaveChangesAsync();
        return true;
    }
}
