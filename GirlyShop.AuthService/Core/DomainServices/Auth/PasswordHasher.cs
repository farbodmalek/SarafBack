using GirlyShop.AuthService.Core.Interfaces;

namespace GirlyShop.AuthService.Core.DomainServices.Auth;

// BCrypt = «آسیاب یک‌طرفه»: رمز را به یک رشته درهم‌ریخته تبدیل می‌کند
// که برگشتش به رمز اصلی تقریباً غیرممکن است.
// خودش یک «Salt» تصادفی می‌سازد و داخل همان رشته نگه می‌دارد،
// پس دو نفر با رمز یکسان، هش متفاوت دارند.
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string plainPassword)
        => BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 12);

    public bool Verify(string plainPassword, string passwordHash)
        => BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
}
