using System.Text;
using Dapper;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Domain;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.Dappers;

namespace ShopMicroservice.Core.DomainServices.Orders
{
    /// <summary>سمت Read سفارش‌ها (Dapper). وضعیت‌ها به‌صورت حروف کوچک (processing, shipped, ...) برمی‌گردند.</summary>
    public class OrderRepositoryRead : IOrderRepositoryRead
    {
        private readonly DapperContext _context;

        public OrderRepositoryRead(DapperContext context)
        {
            _context = context;
        }

        public async Task<List<OrderListItemDto>> GetUserOrdersAsync(int userId, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT o.OrderNumber, o.CreatedAt, o.Total, LOWER(o.Status) AS Status,
                       (SELECT ISNULL(SUM(i.Quantity), 0) FROM OrderItems i WHERE i.OrderId = o.Id) AS ItemsCount
                FROM Orders o
                WHERE o.UserId = @UserId
                ORDER BY o.CreatedAt DESC, o.Id DESC;";

            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<OrderListItemDto>(
                new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
            return rows.ToList();
        }

        public Task<OrderDetailDto?> GetUserOrderAsync(int userId, string orderNumber, CancellationToken cancellationToken)
            => GetDetailAsync(orderNumber, userId, cancellationToken);

        public Task<OrderDetailDto?> GetAdminOrderAsync(string orderNumber, CancellationToken cancellationToken)
            => GetDetailAsync(orderNumber, null, cancellationToken);

        private async Task<OrderDetailDto?> GetDetailAsync(
            string orderNumber, int? userId, CancellationToken cancellationToken)
        {
            var userFilter = userId.HasValue ? " AND o.UserId = @UserId " : " ";

            var sql = $@"
                SELECT o.OrderNumber, o.CreatedAt, LOWER(o.Status) AS Status,
                       o.RecipientName, o.RecipientPhone, o.ShippingAddress,
                       o.ShippingMethod, o.PaymentMethod,
                       o.Subtotal, o.ShippingCost, o.Total,
                       u.FullName AS CustomerName, u.PhoneNumber AS CustomerPhone
                FROM Orders o
                LEFT JOIN Users u ON u.Id = o.UserId
                WHERE o.OrderNumber = @OrderNumber {userFilter};

                SELECT i.ProductId, i.ProductName, i.VariantDescription, i.Sku,
                       i.UnitPrice, i.Quantity, i.ImageUrl
                FROM OrderItems i
                INNER JOIN Orders o ON o.Id = i.OrderId
                WHERE o.OrderNumber = @OrderNumber {userFilter}
                ORDER BY i.Id;";

            using var connection = _context.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(sql, new { OrderNumber = orderNumber, UserId = userId },
                    cancellationToken: cancellationToken));

            var order = await multi.ReadFirstOrDefaultAsync<OrderDetailDto>();
            if (order is null)
            {
                return null;
            }

            order.Items = (await multi.ReadAsync<OrderItemDto>()).ToList();
            order.ShippingMethodTitle = OrderOptions.ShippingMethods.TryGetValue(order.ShippingMethod, out var ship)
                ? ship.Title
                : order.ShippingMethod;
            order.PaymentMethodTitle = OrderOptions.PaymentMethods.TryGetValue(order.PaymentMethod, out var pay)
                ? pay
                : order.PaymentMethod;
            return order;
        }

        public async Task<PagedResult<OrderListItemDto>> GetAdminOrdersAsync(
            OrderFilterDto filter, CancellationToken cancellationToken)
        {
            var where = new StringBuilder(" WHERE 1 = 1 ");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filter.Status)
                && Enum.TryParse<OrderStatus>(filter.Status, true, out var status))
            {
                where.Append(" AND o.Status = @Status ");
                parameters.Add("Status", status.ToString());
            }
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                where.Append(" AND (o.OrderNumber LIKE @Search OR u.FullName LIKE @Search OR u.PhoneNumber LIKE @Search) ");
                parameters.Add("Search", $"%{filter.SearchTerm}%");
            }

            var pageSize = filter.PageSize <= 0 ? 20 : Math.Min(filter.PageSize, 100);
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;

            var countSql = $@"SELECT COUNT(1) FROM Orders o LEFT JOIN Users u ON u.Id = o.UserId {where}";
            var dataSql = $@"
                SELECT o.OrderNumber, o.CreatedAt, o.Total, LOWER(o.Status) AS Status,
                       (SELECT ISNULL(SUM(i.Quantity), 0) FROM OrderItems i WHERE i.OrderId = o.Id) AS ItemsCount,
                       u.FullName AS CustomerName, u.PhoneNumber AS CustomerPhone
                FROM Orders o
                LEFT JOIN Users u ON u.Id = o.UserId
                {where}
                ORDER BY o.CreatedAt DESC, o.Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            parameters.Add("Offset", (pageNumber - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            using var connection = _context.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
            var items = await connection.QueryAsync<OrderListItemDto>(
                new CommandDefinition(dataSql, parameters, cancellationToken: cancellationToken));

            return new PagedResult<OrderListItemDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
