using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ShopMicroservice.Core.Domain;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.EntityFramework;

namespace ShopMicroservice.Core.DomainServices.Orders
{
    /// <summary>سمت Write سفارش‌ها (EF Core) — شامل کم/زیاد کردن موجودی به‌صورت اتمی.</summary>
    public class OrderRepositoryWrite : IOrderRepositoryWrite
    {
        private readonly ShopDbContext _db;

        public OrderRepositoryWrite(ShopDbContext db)
        {
            _db = db;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
            => _db.Database.BeginTransactionAsync(cancellationToken);

        public async Task<List<OrderLineSource>> GetCartLinesAsync(int userId, CancellationToken cancellationToken)
        {
            var lines = await (from ci in _db.CartItems
                               join v in _db.ProductVariants on ci.VariantId equals v.Id
                               join p in _db.Products on v.ProductId equals p.Id
                               where ci.UserId == userId
                               orderby ci.CreatedAt, ci.Id
                               select new OrderLineSource
                               {
                                   VariantId = v.Id,
                                   ProductId = p.Id,
                                   ProductName = p.Name,
                                   Sku = v.Sku,
                                   UnitPrice = v.Price,
                                   Quantity = ci.Quantity,
                                   IsAvailable = v.IsActive && p.Status == ProductStatus.Published,
                                   ImageUrl = _db.ProductImages
                                       .Where(i => i.ProductId == p.Id && i.IsPrimary)
                                       .Select(i => i.ImageUrl)
                                       .FirstOrDefault()
                               }).ToListAsync(cancellationToken);

            if (lines.Count == 0)
            {
                return lines;
            }

            var variantIds = lines.Select(l => l.VariantId).ToList();
            var attributes = await _db.ProductVariantAttributes
                .Where(a => variantIds.Contains(a.VariantId))
                .ToListAsync(cancellationToken);

            foreach (var line in lines)
            {
                line.VariantDescription = string.Join(" / ", attributes
                    .Where(a => a.VariantId == line.VariantId)
                    .OrderBy(a => a.AttributeName)
                    .Select(a => $"{a.AttributeName} {a.AttributeValue}"));
            }

            return lines;
        }

        public async Task<bool> TryReserveStockAsync(Guid variantId, int quantity, CancellationToken cancellationToken)
        {
            var affected = await _db.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE ProductVariants SET StockQuantity = StockQuantity - {quantity}
                   WHERE Id = {variantId} AND StockQuantity >= {quantity}",
                cancellationToken);
            return affected > 0;
        }

        public async Task ReleaseStockAsync(Guid variantId, int quantity, CancellationToken cancellationToken)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE ProductVariants SET StockQuantity = StockQuantity + {quantity} WHERE Id = {variantId}",
                cancellationToken);
        }

        public async Task AddAsync(Order order, CancellationToken cancellationToken)
            => await _db.Orders.AddAsync(order, cancellationToken);

        public Task<Order?> GetTrackedByNumberAsync(string orderNumber, CancellationToken cancellationToken)
            => _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);

        public Task ClearCartAsync(int userId, CancellationToken cancellationToken)
            => _db.CartItems.Where(c => c.UserId == userId).ExecuteDeleteAsync(cancellationToken);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => _db.SaveChangesAsync(cancellationToken);
    }
}
