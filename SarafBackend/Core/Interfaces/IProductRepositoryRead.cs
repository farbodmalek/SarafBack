using ShopMicroservice.Application.DTO;

namespace ShopMicroservice.Core.Interfaces
{
    /// <summary>
    /// سمت Read در CQRS — با Dapper پیاده‌سازی می‌شود؛ فقط SELECT خام و بهینه،
    /// بدون هیچ منطق نوشتنی یا Tracking.
    /// </summary>
    public interface IProductRepositoryRead
    {
        Task<ProductDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<PagedResult<ProductListItemDto>> GetComprehensiveListAsync(
            ProductFilterDto filter, CancellationToken cancellationToken);

        Task<PagedResult<ProductListItemDto>> GetListByCategoryAsync(
            long categoryId, ProductFilterDto filter, CancellationToken cancellationToken);

        Task<PagedResult<ProductListItemDto>> GetFeaturedListAsync(
            ProductFilterDto filter, CancellationToken cancellationToken);
    }
}
