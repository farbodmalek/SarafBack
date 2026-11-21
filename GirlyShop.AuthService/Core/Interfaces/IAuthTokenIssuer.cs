using GirlyShop.AuthService.Application.DTO;

namespace GirlyShop.AuthService.Core.Interfaces;

// هر جا لازم باشد «کارت ورود + کلید رزرو» بدهیم (ثبت‌نام، ورود، رفرش)، از این استفاده می‌کنیم
public interface IAuthTokenIssuer
{
    Task<AuthResponseDto> IssueAsync(UserInfoDto user, string? ipAddress, CancellationToken cancellationToken);
}
