using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using GirlyShopBackend.Core.Domain.ViewModel.Category;
using MediatR;

namespace GirlyShopBackend.Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler
    : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryViewModel>>
{
    private readonly ICategoryReadRepository _readRepository;

    public GetAllCategoriesQueryHandler(ICategoryReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IEnumerable<CategoryViewModel>> Handle(
        GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _readRepository.GetAllAsync();

        return categories.Select(c => new CategoryViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            ImageUrl = c.ImageUrl,
            ParentCategoryId = c.ParentCategoryId,
            ProductCount = c.Products.Count
        });
    }
}
