namespace GirlyShop.AuthService.Core.Domain;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }

    // هیچ‌وقت رمز خام اینجا نمی‌آید؛ فقط نسخه هش‌شده (BCrypt)
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
