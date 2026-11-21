using Dapper;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.Dappers;

namespace ShopMicroservice.Core.DomainServices.Categories
{
    /// <summary>سمت Read دسته‌بندی‌ها (Dapper).</summary>
    public class CategoryRepositoryRead : ICategoryRepositoryRead
    {
        private readonly DapperContext _context;

        public CategoryRepositoryRead(DapperContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT c.Id, c.ParentId, c.Name, c.Slug, c.IsActive,
                       (SELECT COUNT(1) FROM Products p
                        WHERE p.CategoryId = c.Id AND p.Status <> 'Archived') AS ProductCount
                FROM Categories c
                WHERE (@IncludeInactive = 1 OR c.IsActive = 1)
                ORDER BY c.Id;";

            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<CategoryDto>(
                new CommandDefinition(sql, new { IncludeInactive = includeInactive ? 1 : 0 },
                    cancellationToken: cancellationToken));
            return rows.ToList();
        }
    }
}
