using GirlyShopBackend.Core.Domain.ViewModel.Category;

namespace GirlyShopBackend.Core.DomainServices.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryViewModel>> GetAllAsync();
    Task<int> CreateAsync(CategoryCreateRequest request);
    Task<bool> DeleteAsync(int id);
}
