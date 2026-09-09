using GirlyShopBackend.Core.Domain.Entities;
using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace GirlyShopBackend.Infrastructure.Persistence.EntityFramework.Repositories;

// AsNoTracking روی همه کوئری‌ها — چون فقط می‌خونیم، EF Core لازم نیست
// تغییرات این موجودیت‌ها رو ردیابی کنه، این باعث سریع‌تر شدن کوئری می‌شه
public class CategoryReadRepository : ICategoryReadRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        return await _context.Categories.AsNoTracking().AnyAsync(c => c.Slug == slug);
    }
}
