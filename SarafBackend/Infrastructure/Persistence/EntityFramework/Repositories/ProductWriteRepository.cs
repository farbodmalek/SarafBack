using GirlyShopBackend.Core.Domain.Entities;
using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace GirlyShopBackend.Infrastructure.Persistence.EntityFramework.Repositories;

public class ProductWriteRepository : IProductWriteRepository
{
    private readonly ApplicationDbContext _context;

    public ProductWriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdForUpdateAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Variants)
                .ThenInclude(v => v.AttributeValues)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Product product) => await _context.Products.AddAsync(product);

    public void Update(Product product) => _context.Products.Update(product);

    public void Remove(Product product) => _context.Products.Remove(product);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
