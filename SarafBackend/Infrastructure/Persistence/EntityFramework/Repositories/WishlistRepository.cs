using GirlyShopBackend.Core.Domain.Entities;
using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace GirlyShopBackend.Infrastructure.Persistence.EntityFramework.Repositories;

public class WishlistRepository : GenericRepository<WishlistItem>, IWishlistRepository
{
    public WishlistRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<WishlistItem>> GetByUserIdAsync(string userId)
    {
        return await _context.WishlistItems
            .Include(w => w.Product)
                .ThenInclude(p => p.Images)
            .Where(w => w.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string userId, int productId)
    {
        return await _context.WishlistItems
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
    }
}
