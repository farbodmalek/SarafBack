using GirlyShopBackend.Core.Domain.Entities;

namespace GirlyShopBackend.Core.Domain.RepositoryInterfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    // متدهای اختصاصی محصول که در Generic Repository نیستند
    Task<Product?> GetBySlugAsync(string slug);
    Task<Product?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Product>> GetByCategorySlugAsync(string categorySlug);
    Task<bool> SlugExistsAsync(string slug);
}
