using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using MediatR;

namespace GirlyShopBackend.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryWriteRepository _writeRepository;

    public DeleteCategoryCommandHandler(ICategoryWriteRepository writeRepository)
    {
        _writeRepository = writeRepository;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _writeRepository.GetByIdForUpdateAsync(request.Id);
        if (category is null) return false;

        _writeRepository.Remove(category);
        await _writeRepository.SaveChangesAsync();
        return true;
    }
}
