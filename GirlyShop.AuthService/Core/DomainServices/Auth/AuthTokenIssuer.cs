using GirlyShop.AuthService.Application.DTO;
using GirlyShop.AuthService.Core.Domain;
using GirlyShop.AuthService.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace GirlyShop.AuthService.Core.DomainServices.Auth;

public class AuthTokenIssuer : IAuthTokenIssuer
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshTokenRepositoryWrite _refreshTokenWrite;
    private readonly JwtSettings _settings;

    public AuthTokenIssuer(
        ITokenGenerator tokenGenerator,
        IRefreshTokenRepositoryWrite refreshTokenWrite,
        IOptions<JwtSettings> options)
    {
        _tokenGenerator = tokenGenerator;
        _refreshTokenWrite = refreshTokenWrite;
        _settings = options.Value;
    }

    public async Task<AuthResponseDto> IssueAsync(
        UserInfoDto user, string? ipAddress, CancellationToken cancellationToken)
    {
        // ۱) کارت ورود (۱۵ دقیقه‌ای)
        var (accessToken, accessExpiresAt) = _tokenGenerator.GenerateAccessToken(user);

        // ۲) کلید رزرو (۷ روزه) — نسخه خام را فقط به کاربر می‌دهیم، هش آن را ذخیره می‌کنیم
        var rawRefreshToken = _tokenGenerator.GenerateRefreshToken();
        var refreshExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays);

        await _refreshTokenWrite.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _tokenGenerator.HashToken(rawRefreshToken),
            ExpiresAt = refreshExpiresAt,
            CreatedByIp = ipAddress
        }, cancellationToken);
        await _refreshTokenWrite.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            User = user,
            Tokens = new TokenPairDto
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessExpiresAt,
                RefreshToken = rawRefreshToken,
                RefreshTokenExpiresAt = refreshExpiresAt
            }
        };
    }
}
