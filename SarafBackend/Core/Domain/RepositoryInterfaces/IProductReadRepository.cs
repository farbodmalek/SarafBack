using GirlyShopBackend.Core.Domain.Entities;

namespace GirlyShopBackend.Core.Domain.RepositoryInterfaces;

public interface IProductReadRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetByCategorySlugAsync(string categorySlug);
    Task<Product?> GetBySlugAsync(string slug);
    Task<bool> SlugExistsAsync(string slug);
}
