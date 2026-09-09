namespace GirlyShopBackend.Core.Domain.RepositoryInterfaces;

// عملیات پایه‌ای مشترک بین همه Repository ها
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<int> SaveChangesAsync();
}
