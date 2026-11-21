using GirlyShop.AuthService.Application.DTO;
using GirlyShop.AuthService.Core.Exceptions;
using GirlyShop.AuthService.Core.Interfaces;
using MediatR;

namespace GirlyShop.AuthService.Application.Commands.Auth;

// وقتی «کارت ورود» ۱۵ دقیقه‌ای تمام شد، فرانت با «کلید رزرو» یک کارت تازه می‌گیرد
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IRefreshTokenRepositoryWrite _refreshTokenWrite;
    private readonly IUserRepositoryRead _userRead;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IAuthTokenIssuer _tokenIssuer;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepositoryWrite refreshTokenWrite,
        IUserRepositoryRead userRead,
        ITokenGenerator tokenGenerator,
        IAuthTokenIssuer tokenIssuer)
    {
        _refreshTokenWrite = refreshTokenWrite;
        _userRead = userRead;
        _tokenGenerator = tokenGenerator;
        _tokenIssuer = tokenIssuer;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var hash = _tokenGenerator.HashToken(request.RefreshToken);
        var stored = await _refreshTokenWrite.GetByHashAsync(hash, cancellationToken);

        if (stored is null || !stored.IsActive)
        {
            throw new AuthenticationFailedException("نشست شما منقضی شده است. لطفاً دوباره وارد شوید.");
        }

        var user = await _userRead.GetByIdAsync(stored.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            throw new AuthenticationFailedException("نشست شما منقضی شده است. لطفاً دوباره وارد شوید.");
        }

        // کلید رزرو قدیمی یک‌بار مصرف است: باطلش می‌کنیم و یکی تازه می‌دهیم
        stored.RevokedAt = DateTime.UtcNow;
        await _refreshTokenWrite.SaveChangesAsync(cancellationToken);

        return await _tokenIssuer.IssueAsync(user.ToInfo(), request.IpAddress, cancellationToken);
    }
}
