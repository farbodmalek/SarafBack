using Microsoft.AspNetCore.Identity;

namespace GirlyShopBackend.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
