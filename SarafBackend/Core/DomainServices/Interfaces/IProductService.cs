using GirlyShopBackend.Core.Domain.ViewModel.Product;

namespace GirlyShopBackend.Core.DomainServices.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductListItemViewModel>> GetAllAsync();
    Task<IEnumerable<ProductListItemViewModel>> GetByCategorySlugAsync(string categorySlug);
    Task<ProductDetailViewModel?> GetBySlugAsync(string slug);
    Task<int> CreateAsync(ProductCreateRequest request);
    Task<bool> UpdateAsync(int id, ProductCreateRequest request);
    Task<bool> DeleteAsync(int id);
}
