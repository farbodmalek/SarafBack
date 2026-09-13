using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopMicroservice.Core.DomainServices.Product;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.Dappers;
using ShopMicroservice.Infrastructure.Persistence.EntityFramework;

namespace ShopMicroservice.Infrastructure.DependencyInjection
{
    public static class ProductServicesInjection
    {
        /// <summary>
        /// همه‌چیز مربوط به ماژول Product (Read/Write Repository، DbContext، Dapper،
        /// AutoMapper و MediatR handlerها) را در یک متد ثبت می‌کند تا Program.cs
        /// تمیز بماند — دقیقاً همان الگوی HomeServicesInjection.
        /// </summary>
        public static IServiceCollection AddProductServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // ---- سمت Write: Entity Framework ----
            services.AddDbContext<ShopDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ShopWriteConnection")
                    ?? configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IProductRepositoryWrite, ProductRepositoryWrite>();

            // ---- سمت Read: Dapper ----
            services.AddSingleton<DapperContext>();
            services.AddScoped<IProductRepositoryRead, ProductRepositoryRead>();

            // ---- AutoMapper ----
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // ---- MediatR (Command/Query Handlerها) ----
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
