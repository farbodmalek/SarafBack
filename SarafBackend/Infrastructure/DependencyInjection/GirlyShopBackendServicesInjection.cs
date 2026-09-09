using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using GirlyShopBackend.Infrastructure.Persistence.Dappers;
using GirlyShopBackend.Infrastructure.Persistence.EntityFramework;
using GirlyShopBackend.Infrastructure.Persistence.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GirlyShopBackend.Infrastructure.DependencyInjection;

public static class GirlyShopBackendServicesInjection
{
    public static IServiceCollection AddGirlyShopServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ---------- دیتابیس ----------
        services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }));

        // ---------- Dapper (برای گزارش‌های آینده) ----------
        services.AddScoped<DapperContext>();

        // ---------- MediatR (اسکن همه Command/Query Handler ها در این اسمبلی) ----------
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // ---------- Repository های Category (Read/Write جدا) ----------
        services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
        services.AddScoped<ICategoryWriteRepository, CategoryWriteRepository>();

        // ---------- Repository های Product (Read/Write جدا) ----------
        services.AddScoped<IProductReadRepository, ProductReadRepository>();
        services.AddScoped<IProductWriteRepository, ProductWriteRepository>();

        // برای Order, Address, Wishlist, Coupon, Review هم به همین شکل
        // یک ReadRepository و یک WriteRepository جدا اضافه کن (توضیح کامل در README)

        // ---------- CORS برای فرانت Next.js ----------
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }
}
