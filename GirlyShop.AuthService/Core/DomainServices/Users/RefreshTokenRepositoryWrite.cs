using GirlyShop.AuthService.Core.Domain;
using GirlyShop.AuthService.Core.Interfaces;
using GirlyShop.AuthService.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace GirlyShop.AuthService.Core.DomainServices.Users;

public class RefreshTokenRepositoryWrite : IRefreshTokenRepositoryWrite
{
    private readonly AuthDbContext _context;

    public RefreshTokenRepositoryWrite(AuthDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken)
        => await _context.RefreshTokens.AddAsync(token, cancellationToken);

    // با Tracking، چون ممکن است بعدش ابطالش کنیم
    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken)
        => _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => _context.SaveChangesAsync(cancellationToken);
}
