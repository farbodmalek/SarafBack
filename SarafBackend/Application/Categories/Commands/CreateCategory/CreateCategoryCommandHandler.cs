using GirlyShopBackend.Core.Domain.Entities;
using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using GirlyShopBackend.Core.DomainServices;
using MediatR;

namespace GirlyShopBackend.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
{
    private readonly ICategoryWriteRepository _writeRepository;

    public CreateCategoryCommandHandler(ICategoryWriteRepository writeRepository)
    {
        _writeRepository = writeRepository;
    }

    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name,
            Slug = SlugGenerator.Generate(request.Name),
            ParentCategoryId = request.ParentCategoryId,
            ImageUrl = request.ImageUrl
        };

        await _writeRepository.AddAsync(category);
        await _writeRepository.SaveChangesAsync();

        return category.Id;
    }
}
