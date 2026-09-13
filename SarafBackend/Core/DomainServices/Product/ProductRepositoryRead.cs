using System.Text;
using Dapper;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.Dappers;

namespace ShopMicroservice.Core.DomainServices.Product
{
    /// <summary>
    /// سمت Read در CQRS. مستقیم روی جداول Products / ProductVariants /
    /// ProductImages (همان اسکیمای فروشگاه) با SQL خام کار می‌کند.
    /// </summary>
    public class ProductRepositoryRead : IProductRepositoryRead
    {
        private readonly DapperContext _context;

        public ProductRepositoryRead(DapperContext context)
        {
            _context = context;
        }

        public async Task<ProductDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT p.Id, p.Name, p.Slug, p.Description, p.BasePrice, p.Status,
                       p.CategoryId, c.Name AS CategoryName,
                       p.BrandId, b.Name AS BrandName, p.CreatedAt
                FROM Products p
                INNER JOIN Categories c ON c.Id = p.CategoryId
                LEFT JOIN Brands b ON b.Id = p.BrandId
                WHERE p.Id = @Id;

                SELECT ImageUrl FROM ProductImages WHERE ProductId = @Id ORDER BY SortOrder;

                SELECT Id, Sku, Price, CompareAtPrice, StockQuantity
                FROM ProductVariants WHERE ProductId = @Id;";

            using var connection = _context.CreateConnection();
            var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);

            using var multi = await connection.QueryMultipleAsync(command);

            var product = await multi.ReadFirstOrDefaultAsync<ProductDetailDto>();
            if (product is null)
            {
                return null;
            }

            product.ImageUrls = (await multi.ReadAsync<string>()).ToList();
            var variants = (await multi.ReadAsync<ProductVariantDto>()).ToList();

            if (variants.Count > 0)
            {
                var variantIds = variants.Select(v => v.Id).ToList();
                const string attrSql = @"
                    SELECT VariantId, AttributeName, AttributeValue
                    FROM ProductVariantAttributes
                    WHERE VariantId IN @VariantIds;";

                var attrRows = await connection.QueryAsync(
                    new CommandDefinition(attrSql, new { VariantIds = variantIds }, cancellationToken: cancellationToken));

                var byVariant = attrRows
                    .GroupBy(r => (Guid)r.VariantId)
                    .ToDictionary(g => g.Key, g => g.ToDictionary(
                        r => (string)r.AttributeName, r => (string)r.AttributeValue));

                foreach (var variant in variants)
                {
                    if (byVariant.TryGetValue(variant.Id, out var attrs))
                    {
                        variant.Attributes = attrs;
                    }
                }
            }

            product.Variants = variants;
            return product;
        }

        public Task<PagedResult<ProductListItemDto>> GetComprehensiveListAsync(
            ProductFilterDto filter, CancellationToken cancellationToken)
            => QueryListAsync(filter, categoryId: null, featuredOnly: false, cancellationToken);

        public Task<PagedResult<ProductListItemDto>> GetListByCategoryAsync(
            long categoryId, ProductFilterDto filter, CancellationToken cancellationToken)
            => QueryListAsync(filter, categoryId, featuredOnly: false, cancellationToken);

        public Task<PagedResult<ProductListItemDto>> GetFeaturedListAsync(
            ProductFilterDto filter, CancellationToken cancellationToken)
            => QueryListAsync(filter, categoryId: null, featuredOnly: true, cancellationToken);

        private async Task<PagedResult<ProductListItemDto>> QueryListAsync(
            ProductFilterDto filter, long? categoryId, bool featuredOnly, CancellationToken cancellationToken)
        {
            var where = new StringBuilder(" WHERE p.Status = 'Published' ");
            var parameters = new DynamicParameters();

            if (categoryId.HasValue)
            {
                where.Append(" AND p.CategoryId = @CategoryId ");
                parameters.Add("CategoryId", categoryId.Value);
            }
            else if (filter.CategoryId.HasValue)
            {
                where.Append(" AND p.CategoryId = @CategoryId ");
                parameters.Add("CategoryId", filter.CategoryId.Value);
            }
            if (featuredOnly)
            {
                where.Append(" AND p.IsFeatured = 1 ");
            }
            if (filter.BrandId.HasValue)
            {
                where.Append(" AND p.BrandId = @BrandId ");
                parameters.Add("BrandId", filter.BrandId.Value);
            }
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                where.Append(" AND p.Name LIKE @SearchTerm ");
                parameters.Add("SearchTerm", $"%{filter.SearchTerm}%");
            }
            if (filter.MinPrice.HasValue)
            {
                where.Append(" AND p.BasePrice >= @MinPrice ");
                parameters.Add("MinPrice", filter.MinPrice.Value);
            }
            if (filter.MaxPrice.HasValue)
            {
                where.Append(" AND p.BasePrice <= @MaxPrice ");
                parameters.Add("MaxPrice", filter.MaxPrice.Value);
            }
            if (filter.InStockOnly == true)
            {
                where.Append(@" AND EXISTS (
                    SELECT 1 FROM ProductVariants v
                    WHERE v.ProductId = p.Id AND v.StockQuantity > 0) ");
            }

            var pageSize = filter.PageSize <= 0 ? 20 : filter.PageSize;
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var offset = (pageNumber - 1) * pageSize;

            var countSql = $"SELECT COUNT(1) FROM Products p {where}";

            var dataSql = $@"
                SELECT p.Id, p.Name, p.Slug, p.BasePrice, p.IsFeatured,
                       c.Name AS CategoryName, b.Name AS BrandName,
                       (SELECT TOP 1 i.ImageUrl FROM ProductImages i
                        WHERE i.ProductId = p.Id AND i.IsPrimary = 1) AS PrimaryImageUrl
                FROM Products p
                INNER JOIN Categories c ON c.Id = p.CategoryId
                LEFT JOIN Brands b ON b.Id = p.BrandId
                {where}
                ORDER BY p.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            using var connection = _context.CreateConnection();

            var totalCount = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

            var items = await connection.QueryAsync<ProductListItemDto>(
                new CommandDefinition(dataSql, parameters, cancellationToken: cancellationToken));

            return new PagedResult<ProductListItemDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
