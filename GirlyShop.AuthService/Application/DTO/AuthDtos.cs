using System.ComponentModel.DataAnnotations;

namespace GirlyShop.AuthService.Application.DTO;

// ---------- ورودی‌ها (چیزی که فرانت می‌فرستد) ----------

public class RegisterRequestDto
{
    [Required(ErrorMessage = "نام را وارد کنید")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "نام باید حداقل ۳ کاراکتر باشد")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "شماره موبایل را وارد کنید")]
    [RegularExpression(@"^09[0-9]{9}$", ErrorMessage = "شماره موبایل معتبر نیست")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور را وارد کنید")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز عبور باید حداقل ۶ کاراکتر باشد")]
    public string Password { get; set; } = string.Empty;
}

public class LoginRequestDto
{
    [Required(ErrorMessage = "شماره موبایل را وارد کنید")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور را وارد کنید")]
    public string Password { get; set; } = string.Empty;
}

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

// ---------- خروجی‌ها (چیزی که به فرانت برمی‌گردد) ----------

public class UserInfoDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
}

public class TokenPairDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAt { get; set; }
}

// شکل خروجی دقیقاً با چیزی که فرانت‌اند (LoginResponse) انتظار دارد هماهنگ است
public class AuthResponseDto
{
    public UserInfoDto User { get; set; } = new();
    public TokenPairDto Tokens { get; set; } = new();
}

// ---------- فقط داخل بک‌اند (شامل PasswordHash؛ هیچ‌وقت به بیرون برنمی‌گردد) ----------

public class UserAuthDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }

    public UserInfoDto ToInfo() => new()
    {
        Id = Id,
        FullName = FullName,
        Phone = PhoneNumber,
        Email = Email,
        IsAdmin = IsAdmin
    };
}
