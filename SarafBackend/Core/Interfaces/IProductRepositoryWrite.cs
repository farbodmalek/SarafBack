using ShopMicroservice.Core.Domain;

namespace ShopMicroservice.Core.Interfaces
{
    /// <summary>
    /// سمت Write در CQRS — با Entity Framework پیاده‌سازی می‌شود.
    /// هرگونه Insert/Update/Delete فقط از این مسیر انجام می‌شود.
    /// </summary>
    public interface IProductRepositoryWrite
    {
        Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Product product, CancellationToken cancellationToken);
        Task UpdateAsync(Product product, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
