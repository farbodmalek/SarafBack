using GirlyShop.AuthService.Core.Domain;

namespace GirlyShop.AuthService.Core.Interfaces;

// سمت Write — با Entity Framework
public interface IUserRepositoryWrite
{
    Task<bool> PhoneExistsAsync(string phoneNumber, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
