using GirlyShopBackend.Core.Domain.Entities;
using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace GirlyShopBackend.Infrastructure.Persistence.EntityFramework.Repositories;

public class CategoryWriteRepository : ICategoryWriteRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryWriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // برخلاف Read Repository، اینجا Tracking لازمه چون قراره Update بشه
    public async Task<Category?> GetByIdForUpdateAsync(int id)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Category category) => await _context.Categories.AddAsync(category);

    public void Update(Category category) => _context.Categories.Update(category);

    public void Remove(Category category) => _context.Categories.Remove(category);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
