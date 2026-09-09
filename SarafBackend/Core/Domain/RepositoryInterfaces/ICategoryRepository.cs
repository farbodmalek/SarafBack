using GirlyShopBackend.Core.Domain.Entities;

namespace GirlyShopBackend.Core.Domain.RepositoryInterfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug);
    Task<IEnumerable<Category>> GetTopLevelWithChildrenAsync();
}
