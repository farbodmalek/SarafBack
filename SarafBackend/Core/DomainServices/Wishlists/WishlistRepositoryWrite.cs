using Microsoft.EntityFrameworkCore;
using ShopMicroservice.Core.Domain;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.EntityFramework;

namespace ShopMicroservice.Core.DomainServices.Wishlists
{
    /// <summary>سمت Write علاقه‌مندی‌ها (EF Core).</summary>
    public class WishlistRepositoryWrite : IWishlistRepositoryWrite
    {
        private readonly ShopDbContext _db;

        public WishlistRepositoryWrite(ShopDbContext db)
        {
            _db = db;
        }

        public Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken)
            => _db.Products.AnyAsync(
                p => p.Id == productId && p.Status == ProductStatus.Published, cancellationToken);

        public Task<bool> ExistsAsync(int userId, Guid productId, CancellationToken cancellationToken)
            => _db.WishlistItems.AnyAsync(
                w => w.UserId == userId && w.ProductId == productId, cancellationToken);

        public async Task AddAsync(WishlistItem item, CancellationToken cancellationToken)
            => await _db.WishlistItems.AddAsync(item, cancellationToken);

        public Task RemoveAsync(int userId, Guid productId, CancellationToken cancellationToken)
            => _db.WishlistItems
                .Where(w => w.UserId == userId && w.ProductId == productId)
                .ExecuteDeleteAsync(cancellationToken);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => _db.SaveChangesAsync(cancellationToken);
    }
}
