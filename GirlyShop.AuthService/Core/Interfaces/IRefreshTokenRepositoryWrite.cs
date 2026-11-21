using GirlyShop.AuthService.Core.Domain;

namespace GirlyShop.AuthService.Core.Interfaces;

public interface IRefreshTokenRepositoryWrite
{
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken);
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
