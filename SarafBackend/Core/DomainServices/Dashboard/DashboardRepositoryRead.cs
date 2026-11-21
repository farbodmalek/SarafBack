using Dapper;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.Dappers;

namespace ShopMicroservice.Core.DomainServices.Dashboard
{
    /// <summary>آمار داشبورد ادمین (Dapper، یک رفت‌وبرگشت به دیتابیس).</summary>
    public class DashboardRepositoryRead : IDashboardRepositoryRead
    {
        private readonly DapperContext _context;

        public DashboardRepositoryRead(DapperContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetAsync(
            DateTime dayStartUtc, DateTime monthStartUtc, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT
                    (SELECT COUNT(1) FROM Products WHERE Status <> 'Archived')                       AS TotalProducts,
                    (SELECT COUNT(1) FROM Orders WHERE CreatedAt >= @DayStart)                       AS TodayOrders,
                    (SELECT COUNT(1) FROM Orders WHERE Status = 'Processing')                        AS PendingOrders,
                    (SELECT COUNT(1) FROM Users)                                                     AS UsersCount,
                    (SELECT ISNULL(SUM(Total), 0) FROM Orders
                     WHERE CreatedAt >= @MonthStart AND Status <> 'Cancelled')                       AS MonthSales;";

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstAsync<DashboardDto>(
                new CommandDefinition(sql,
                    new { DayStart = dayStartUtc, MonthStart = monthStartUtc },
                    cancellationToken: cancellationToken));
        }
    }
}
