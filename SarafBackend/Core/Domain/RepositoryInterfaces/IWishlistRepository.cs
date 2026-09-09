using GirlyShopBackend.Core.Domain.Entities;

namespace GirlyShopBackend.Core.Domain.RepositoryInterfaces;

public interface IWishlistRepository : IGenericRepository<WishlistItem>
{
    Task<IEnumerable<WishlistItem>> GetByUserIdAsync(int userId);
    Task<bool> ExistsAsync(int userId, int productId);
}
