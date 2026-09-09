using GirlyShopBackend.Core.Domain.Entities;

namespace GirlyShopBackend.Core.Domain.RepositoryInterfaces;

public interface IWishlistRepository : IGenericRepository<WishlistItem>
{
    Task<IEnumerable<WishlistItem>> GetByUserIdAsync(string userId);
    Task<bool> ExistsAsync(string userId, int productId);
}
