using Microsoft.EntityFrameworkCore;
using ShopMicroservice.Core.Domain;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.EntityFramework;

namespace ShopMicroservice.Core.DomainServices.Categories
{
    /// <summary>سمت Write دسته‌بندی‌ها (EF Core).</summary>
    public class CategoryRepositoryWrite : ICategoryRepositoryWrite
    {
        private readonly ShopDbContext _db;

        public CategoryRepositoryWrite(ShopDbContext db)
        {
            _db = db;
        }

        public Task<Category?> GetByIdAsync(long id, CancellationToken cancellationToken)
            => _db.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public Task<bool> SlugExistsAsync(string slug, long? exceptId, CancellationToken cancellationToken)
            => _db.Categories.AnyAsync(
                c => c.Slug == slug && (!exceptId.HasValue || c.Id != exceptId.Value), cancellationToken);

        public Task<bool> HasProductsAsync(long id, CancellationToken cancellationToken)
            => _db.Products.AnyAsync(p => p.CategoryId == id, cancellationToken);

        public async Task AddAsync(Category category, CancellationToken cancellationToken)
            => await _db.Categories.AddAsync(category, cancellationToken);

        public void Remove(Category category) => _db.Categories.Remove(category);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => _db.SaveChangesAsync(cancellationToken);
    }
}
