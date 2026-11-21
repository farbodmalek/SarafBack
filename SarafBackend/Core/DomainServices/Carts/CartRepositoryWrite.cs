using Microsoft.EntityFrameworkCore;
using ShopMicroservice.Core.Domain;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.EntityFramework;

namespace ShopMicroservice.Core.DomainServices.Carts
{
    /// <summary>سمت Write سبد خرید (EF Core).</summary>
    public class CartRepositoryWrite : ICartRepositoryWrite
    {
        private readonly ShopDbContext _db;

        public CartRepositoryWrite(ShopDbContext db)
        {
            _db = db;
        }

        public async Task<ProductVariant?> GetPurchasableVariantAsync(Guid variantId, CancellationToken cancellationToken)
        {
            return await (from v in _db.ProductVariants
                          join p in _db.Products on v.ProductId equals p.Id
                          where v.Id == variantId && v.IsActive && p.Status == ProductStatus.Published
                          select v).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<CartItem?> GetItemAsync(int userId, Guid variantId, CancellationToken cancellationToken)
            => _db.CartItems.FirstOrDefaultAsync(
                c => c.UserId == userId && c.VariantId == variantId, cancellationToken);

        public async Task AddAsync(CartItem item, CancellationToken cancellationToken)
            => await _db.CartItems.AddAsync(item, cancellationToken);

        public void Remove(CartItem item) => _db.CartItems.Remove(item);

        public Task ClearAsync(int userId, CancellationToken cancellationToken)
            => _db.CartItems.Where(c => c.UserId == userId).ExecuteDeleteAsync(cancellationToken);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => _db.SaveChangesAsync(cancellationToken);
    }
}
