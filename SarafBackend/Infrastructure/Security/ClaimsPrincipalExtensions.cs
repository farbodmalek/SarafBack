using System.Security.Claims;

namespace ShopMicroservice.Infrastructure.Security
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>شناسه‌ی کاربر لاگین‌شده؛ از claim به نام «sub» داخل توکنی که سرویس Auth ساخته.</summary>
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var sub = user.FindFirst("sub")?.Value;
            if (!int.TryParse(sub, out var userId))
            {
                throw new UnauthorizedAccessException("توکن معتبر نیست.");
            }
            return userId;
        }
    }
}
