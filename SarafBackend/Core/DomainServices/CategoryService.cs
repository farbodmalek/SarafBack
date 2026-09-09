using GirlyShopBackend.Core.Domain.Entities;
using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using GirlyShopBackend.Core.Domain.ViewModel.Category;
using GirlyShopBackend.Core.DomainServices.Interfaces;

namespace GirlyShopBackend.Core.DomainServices;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryViewModel>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(c => new CategoryViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            ParentCategoryId = c.ParentCategoryId,
            ProductCount = c.Products?.Count ?? 0
        });
    }

    public async Task<int> CreateAsync(CategoryCreateRequest request)
    {
        var category = new Category
        {
            Name = request.Name,
            Slug = request.Name.Trim().Replace(" ", "-"),
            ParentCategoryId = request.ParentCategoryId
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return category.Id;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null) return false;

        _categoryRepository.Remove(category);
        await _categoryRepository.SaveChangesAsync();
        return true;
    }
}
