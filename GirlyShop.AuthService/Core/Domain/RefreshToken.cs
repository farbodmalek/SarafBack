namespace GirlyShop.AuthService.Core.Domain;

public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }

    // در دیتابیس فقط «هش» توکن ذخیره می‌شود، نه خود توکن
    // (اگه کسی دیتابیس را ببیند، نمی‌تواند از آن استفاده کند)
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    public string? CreatedByIp { get; set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
}
