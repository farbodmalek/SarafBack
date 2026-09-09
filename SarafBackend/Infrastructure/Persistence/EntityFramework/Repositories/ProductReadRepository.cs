using GirlyShopBackend.Core.Domain.Entities;
using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace GirlyShopBackend.Infrastructure.Persistence.EntityFramework.Repositories;

public class ProductReadRepository : IProductReadRepository
{
    private readonly ApplicationDbContext _context;

    public ProductReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategorySlugAsync(string categorySlug)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive && p.Category.Slug == categorySlug)
            .ToListAsync();
    }

    public async Task<Product?> GetBySlugAsync(string slug)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
                .ThenInclude(v => v.AttributeValues)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        return await _context.Products.AsNoTracking().AnyAsync(p => p.Slug == slug);
    }
}
