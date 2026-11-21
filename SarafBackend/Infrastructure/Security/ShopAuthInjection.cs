using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ShopMicroservice.Infrastructure.Security
{
    /// <summary>
    /// این سرویس توکن نمی‌سازد؛ فقط توکنی را که سرویس Auth ساخته بررسی می‌کند.
    /// بخش «Jwt» (Key، Issuer، Audience) باید دقیقاً با سرویس Auth یکی باشد.
    /// </summary>
    public static class ShopAuthInjection
    {
        public static IServiceCollection AddShopAuthentication(
            this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("Jwt");
            var key = section["Key"] ?? string.Empty;
            var issuer = section["Issuer"] ?? string.Empty;
            var audience = section["Audience"] ?? string.Empty;

            if (Encoding.UTF8.GetByteCount(key) < 32)
            {
                throw new InvalidOperationException(
                    "Jwt:Key تنظیم نشده یا کوتاه است (حداقل ۳۲ کاراکتر) و باید با سرویس Auth یکی باشد. " +
                    "در توسعه از appsettings.Development.json استفاده می‌شود؛ روی سرور با متغیر محیطی Jwt__Key بدهید.");
            }

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // اسم claim ها همان‌طور که در توکن نوشته شده بماند (sub, name, role, ...)
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ClockSkew = TimeSpan.FromSeconds(30),
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });

            services.AddAuthorization();
            return services;
        }
    }
}
