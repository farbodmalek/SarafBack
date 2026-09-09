using GirlyShopBackend.Core.Domain.Entities;

namespace GirlyShopBackend.Core.Domain.RepositoryInterfaces;

public interface IProductWriteRepository
{
    Task<Product?> GetByIdForUpdateAsync(int id);
    Task AddAsync(Product product);
    void Update(Product product);
    void Remove(Product product);
    Task<int> SaveChangesAsync();
}
