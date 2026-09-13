using Microsoft.EntityFrameworkCore;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.EntityFramework;

namespace ShopMicroservice.Core.DomainServices.Product
{
    /// <summary>
    /// سمت Write در CQRS. تمام تغییرات (Insert/Update/Delete) فقط
    /// از طریق این کلاس و با EF Core انجام می‌شود.
    /// </summary>
    public class ProductRepositoryWrite : IProductRepositoryWrite
    {
        private readonly ShopDbContext _dbContext;

        public ProductRepositoryWrite(ShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Domain.Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Products
                .Include(p => p.Images)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task AddAsync(Domain.Product product, CancellationToken cancellationToken)
        {
            await _dbContext.Products.AddAsync(product, cancellationToken);
        }

        public Task UpdateAsync(Domain.Product product, CancellationToken cancellationToken)
        {
            product.UpdatedAt = DateTime.UtcNow;
            _dbContext.Products.Update(product);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (product is not null)
            {
                _dbContext.Products.Remove(product);
            }
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
