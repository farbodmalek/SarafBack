using GirlyShopBackend.Core.Domain.Entities;

namespace GirlyShopBackend.Core.Domain.RepositoryInterfaces;

// فقط عملیات نوشتن — Add/Update/Remove/Save
public interface ICategoryWriteRepository
{
    Task<Category?> GetByIdForUpdateAsync(int id); // با Tracking، برای ویرایش
    Task AddAsync(Category category);
    void Update(Category category);
    void Remove(Category category);
    Task<int> SaveChangesAsync();
}
