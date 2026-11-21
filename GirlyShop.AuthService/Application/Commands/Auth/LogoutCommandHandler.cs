using GirlyShop.AuthService.Core.Interfaces;
using MediatR;

namespace GirlyShop.AuthService.Application.Commands.Auth;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepositoryWrite _refreshTokenWrite;
    private readonly ITokenGenerator _tokenGenerator;

    public LogoutCommandHandler(
        IRefreshTokenRepositoryWrite refreshTokenWrite,
        ITokenGenerator tokenGenerator)
    {
        _refreshTokenWrite = refreshTokenWrite;
        _tokenGenerator = tokenGenerator;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var hash = _tokenGenerator.HashToken(request.RefreshToken);
        var stored = await _refreshTokenWrite.GetByHashAsync(hash, cancellationToken);

        // اگر توکن پیدا نشد یا قبلاً باطل شده بود، مشکلی نیست؛ نتیجه یکی است (کاربر خارج شده)
        if (stored is not null && stored.RevokedAt is null)
        {
            stored.RevokedAt = DateTime.UtcNow;
            await _refreshTokenWrite.SaveChangesAsync(cancellationToken);
        }
    }
}
