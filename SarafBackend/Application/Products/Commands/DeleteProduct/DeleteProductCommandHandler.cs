using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using MediatR;

namespace GirlyShopBackend.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductWriteRepository _writeRepository;

    public DeleteProductCommandHandler(IProductWriteRepository writeRepository)
    {
        _writeRepository = writeRepository;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _writeRepository.GetByIdForUpdateAsync(request.Id);
        if (product is null) return false;

        _writeRepository.Remove(product);
        await _writeRepository.SaveChangesAsync();
        return true;
    }
}
