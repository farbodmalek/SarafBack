using Dapper;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.Dappers;

namespace ShopMicroservice.Core.DomainServices.Wishlists
{
    /// <summary>سمت Read علاقه‌مندی‌ها (Dapper).</summary>
    public class WishlistRepositoryRead : IWishlistRepositoryRead
    {
        private readonly DapperContext _context;

        public WishlistRepositoryRead(DapperContext context)
        {
            _context = context;
        }

        public async Task<List<ProductListItemDto>> GetListAsync(int userId, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT p.Id, p.Name, p.Slug, p.BasePrice, p.IsFeatured,
                       c.Name AS CategoryName, b.Name AS BrandName,
                       (SELECT MIN(v.Price) FROM ProductVariants v
                        WHERE v.ProductId = p.Id AND v.IsActive = 1
                          AND v.CompareAtPrice IS NOT NULL AND v.CompareAtPrice > v.Price) AS DiscountPrice,
                       (SELECT TOP 1 i.ImageUrl FROM ProductImages i
                        WHERE i.ProductId = p.Id AND i.IsPrimary = 1) AS PrimaryImageUrl
                FROM WishlistItems w
                INNER JOIN Products p ON p.Id = w.ProductId
                INNER JOIN Categories c ON c.Id = p.CategoryId
                LEFT JOIN Brands b ON b.Id = p.BrandId
                WHERE w.UserId = @UserId AND p.Status = 'Published'
                ORDER BY w.CreatedAt DESC;";

            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<ProductListItemDto>(
                new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
            return rows.ToList();
        }

        public async Task<List<Guid>> GetProductIdsAsync(int userId, CancellationToken cancellationToken)
        {
            const string sql = "SELECT ProductId FROM WishlistItems WHERE UserId = @UserId;";
            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<Guid>(
                new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
            return rows.ToList();
        }
    }
}
