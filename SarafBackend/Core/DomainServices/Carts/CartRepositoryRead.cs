using Dapper;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.Dappers;

namespace ShopMicroservice.Core.DomainServices.Carts
{
    /// <summary>سمت Read سبد خرید (Dapper). قیمت و موجودی همیشه از جدول واریانت‌ها خوانده می‌شود.</summary>
    public class CartRepositoryRead : ICartRepositoryRead
    {
        private readonly DapperContext _context;

        public CartRepositoryRead(DapperContext context)
        {
            _context = context;
        }

        public async Task<CartDto> GetCartAsync(int userId, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT ci.VariantId, p.Id AS ProductId, p.Name AS Title,
                       (SELECT TOP 1 i.ImageUrl FROM ProductImages i
                        WHERE i.ProductId = p.Id AND i.IsPrimary = 1) AS Image,
                       v.Price, ci.Quantity, v.StockQuantity AS Stock
                FROM CartItems ci
                INNER JOIN ProductVariants v ON v.Id = ci.VariantId
                INNER JOIN Products p ON p.Id = v.ProductId
                WHERE ci.UserId = @UserId
                ORDER BY ci.CreatedAt, ci.Id;

                SELECT ci.VariantId, a.AttributeName AS Name, a.AttributeValue AS Value
                FROM CartItems ci
                INNER JOIN ProductVariantAttributes a ON a.VariantId = ci.VariantId
                WHERE ci.UserId = @UserId
                ORDER BY a.AttributeName;";

            using var connection = _context.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));

            var items = (await multi.ReadAsync<CartItemDto>()).ToList();
            var attrRows = (await multi.ReadAsync<CartAttributeRow>()).ToList();

            foreach (var item in items)
            {
                item.Attributes = attrRows
                    .Where(a => a.VariantId == item.VariantId)
                    .Select(a => new CartAttributeDto { Name = a.Name, Value = a.Value })
                    .ToList();
            }

            return new CartDto { Items = items };
        }

        private class CartAttributeRow
        {
            public Guid VariantId { get; set; }
            public string Name { get; set; } = default!;
            public string Value { get; set; } = default!;
        }
    }
}
