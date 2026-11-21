using GirlyShop.AuthService.Application.DTO;

namespace GirlyShop.AuthService.Core.Interfaces;

public interface ITokenGenerator
{
    // «کارت ورود» کوتاه‌مدت (JWT)
    (string Token, DateTime ExpiresAt) GenerateAccessToken(UserInfoDto user);

    // «کلید رزرو» بلندمدت (یک رشته تصادفی)
    string GenerateRefreshToken();

    // هش کردن توکن برای ذخیره در دیتابیس
    string HashToken(string token);
}
