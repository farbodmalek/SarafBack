using ShopMicroservice.Core.DomainServices.Addresses;
using ShopMicroservice.Core.DomainServices.Carts;
using ShopMicroservice.Core.DomainServices.Categories;
using ShopMicroservice.Core.DomainServices.Dashboard;
using ShopMicroservice.Core.DomainServices.Orders;
using ShopMicroservice.Core.DomainServices.Wishlists;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Infrastructure.DependencyInjection
{
    public static class ShopModulesInjection
    {
        /// <summary>
        /// ماژول‌های فروشگاه (دسته‌بندی، سبد خرید، آدرس، علاقه‌مندی، سفارش، داشبورد).
        /// MediatR و DbContext و Dapper را قبلاً AddProductServices ثبت کرده؛ اینجا فقط Repository ها ثبت می‌شوند.
        /// </summary>
        public static IServiceCollection AddShopModules(this IServiceCollection services)
        {
            services.AddScoped<ICategoryRepositoryRead, CategoryRepositoryRead>();
            services.AddScoped<ICategoryRepositoryWrite, CategoryRepositoryWrite>();

            services.AddScoped<ICartRepositoryRead, CartRepositoryRead>();
            services.AddScoped<ICartRepositoryWrite, CartRepositoryWrite>();

            services.AddScoped<IAddressRepositoryRead, AddressRepositoryRead>();
            services.AddScoped<IAddressRepositoryWrite, AddressRepositoryWrite>();

            services.AddScoped<IWishlistRepositoryRead, WishlistRepositoryRead>();
            services.AddScoped<IWishlistRepositoryWrite, WishlistRepositoryWrite>();

            services.AddScoped<IOrderRepositoryRead, OrderRepositoryRead>();
            services.AddScoped<IOrderRepositoryWrite, OrderRepositoryWrite>();

            services.AddScoped<IDashboardRepositoryRead, DashboardRepositoryRead>();

            return services;
        }
    }
}
