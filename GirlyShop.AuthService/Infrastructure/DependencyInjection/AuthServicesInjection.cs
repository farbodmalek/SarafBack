using System.Text;
using GirlyShop.AuthService.Core.DomainServices.Auth;
using GirlyShop.AuthService.Core.DomainServices.Users;
using GirlyShop.AuthService.Core.Interfaces;
using GirlyShop.AuthService.Infrastructure.Persistence.Dappers;
using GirlyShop.AuthService.Infrastructure.Persistence.EntityFramework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GirlyShop.AuthService.Infrastructure.DependencyInjection;

public static class AuthServicesInjection
{
    public static IServiceCollection AddAuthServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ---------- دیتابیس ----------
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection تنظیم نشده است.");

        services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<DapperContext>();

        // ---------- CQRS ----------
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(AuthServicesInjection).Assembly));

        // ---------- Repository ها ----------
        services.AddScoped<IUserRepositoryRead, UserRepositoryRead>();
        services.AddScoped<IUserRepositoryWrite, UserRepositoryWrite>();
        services.AddScoped<IRefreshTokenRepositoryWrite, RefreshTokenRepositoryWrite>();

        // ---------- هش رمز و توکن ----------
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthTokenIssuer, AuthTokenIssuer>();

        // ---------- بررسی توکن (JWT) ----------
        var jwt = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();

        if (Encoding.UTF8.GetByteCount(jwt.Key) < 32)
        {
            throw new InvalidOperationException(
                "Jwt:Key تنظیم نشده یا کوتاه است (حداقل ۳۲ کاراکتر). " +
                "در محیط توسعه از appsettings.Development.json استفاده می‌شود؛ " +
                "در سرور واقعی آن را با متغیر محیطی Jwt__Key بدهید.");
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
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

        services.AddAuthorization();

        return services;
    }
}
