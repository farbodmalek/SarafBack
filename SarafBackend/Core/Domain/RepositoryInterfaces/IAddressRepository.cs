using GirlyShopBackend.Core.Domain.Entities;

namespace GirlyShopBackend.Core.Domain.RepositoryInterfaces;

public interface IAddressRepository : IGenericRepository<Address>
{
    Task<IEnumerable<Address>> GetByUserIdAsync(int userId);
}
