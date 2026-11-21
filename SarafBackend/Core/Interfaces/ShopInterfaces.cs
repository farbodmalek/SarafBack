using Microsoft.EntityFrameworkCore.Storage;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Domain;

namespace ShopMicroservice.Core.Interfaces
{
    // ====================== دسته‌بندی ======================
    public interface ICategoryRepositoryRead
    {
        Task<List<CategoryDto>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken);
    }

    public interface ICategoryRepositoryWrite
    {
        Task<Category?> GetByIdAsync(long id, CancellationToken cancellationToken);
        Task<bool> SlugExistsAsync(string slug, long? exceptId, CancellationToken cancellationToken);
        Task<bool> HasProductsAsync(long id, CancellationToken cancellationToken);
        Task AddAsync(Category category, CancellationToken cancellationToken);
        void Remove(Category category);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }

    // ====================== سبد خرید ======================
    public interface ICartRepositoryRead
    {
        Task<CartDto> GetCartAsync(int userId, CancellationToken cancellationToken);
    }

    public interface ICartRepositoryWrite
    {
        /// <summary>واریانتی که فعال است و محصولش منتشر شده؛ وگرنه null.</summary>
        Task<ProductVariant?> GetPurchasableVariantAsync(Guid variantId, CancellationToken cancellationToken);
        Task<CartItem?> GetItemAsync(int userId, Guid variantId, CancellationToken cancellationToken);
        Task AddAsync(CartItem item, CancellationToken cancellationToken);
        void Remove(CartItem item);
        Task ClearAsync(int userId, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }

    // ====================== آدرس ======================
    public interface IAddressRepositoryRead
    {
        Task<List<AddressDto>> GetListAsync(int userId, CancellationToken cancellationToken);
        Task<AddressDto?> GetByIdAsync(int userId, int addressId, CancellationToken cancellationToken);
    }

    public interface IAddressRepositoryWrite
    {
        Task<Address?> GetTrackedAsync(int userId, int addressId, CancellationToken cancellationToken);
        Task<int> CountAsync(int userId, CancellationToken cancellationToken);
        Task AddAsync(Address address, CancellationToken cancellationToken);
        void Remove(Address address);
        /// <summary>همه‌ی آدرس‌های کاربر را «غیرپیش‌فرض» می‌کند (به‌جز یکی)</summary>
        Task ClearDefaultAsync(int userId, int? exceptAddressId, CancellationToken cancellationToken);
        /// <summary>قدیمی‌ترین آدرس کاربر را پیش‌فرض می‌کند (بعد از حذف آدرس پیش‌فرض)</summary>
        Task PromoteOldestAsDefaultAsync(int userId, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }

    // ====================== علاقه‌مندی ======================
    public interface IWishlistRepositoryRead
    {
        Task<List<ProductListItemDto>> GetListAsync(int userId, CancellationToken cancellationToken);
        Task<List<Guid>> GetProductIdsAsync(int userId, CancellationToken cancellationToken);
    }

    public interface IWishlistRepositoryWrite
    {
        Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int userId, Guid productId, CancellationToken cancellationToken);
        Task AddAsync(WishlistItem item, CancellationToken cancellationToken);
        Task RemoveAsync(int userId, Guid productId, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }

    // ====================== سفارش ======================
    /// <summary>یک سطر سبد خرید همراه با اطلاعات لازم برای «عکس لحظه‌ی خرید» سفارش.</summary>
    public class OrderLineSource
    {
        public Guid VariantId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string Sku { get; set; } = default!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public string? VariantDescription { get; set; }
    }

    public interface IOrderRepositoryRead
    {
        Task<List<OrderListItemDto>> GetUserOrdersAsync(int userId, CancellationToken cancellationToken);
        Task<OrderDetailDto?> GetUserOrderAsync(int userId, string orderNumber, CancellationToken cancellationToken);
        Task<PagedResult<OrderListItemDto>> GetAdminOrdersAsync(OrderFilterDto filter, CancellationToken cancellationToken);
        Task<OrderDetailDto?> GetAdminOrderAsync(string orderNumber, CancellationToken cancellationToken);
    }

    public interface IOrderRepositoryWrite
    {
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
        Task<List<OrderLineSource>> GetCartLinesAsync(int userId, CancellationToken cancellationToken);
        /// <summary>موجودی را «اتمی» کم می‌کند؛ اگر موجودی کافی نبود false.</summary>
        Task<bool> TryReserveStockAsync(Guid variantId, int quantity, CancellationToken cancellationToken);
        Task ReleaseStockAsync(Guid variantId, int quantity, CancellationToken cancellationToken);
        Task AddAsync(Order order, CancellationToken cancellationToken);
        Task<Order?> GetTrackedByNumberAsync(string orderNumber, CancellationToken cancellationToken);
        Task ClearCartAsync(int userId, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }

    // ====================== داشبورد ======================
    public interface IDashboardRepositoryRead
    {
        Task<DashboardDto> GetAsync(DateTime dayStartUtc, DateTime monthStartUtc, CancellationToken cancellationToken);
    }
}
