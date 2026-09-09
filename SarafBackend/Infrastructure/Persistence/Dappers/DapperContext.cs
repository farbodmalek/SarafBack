using Microsoft.Data.SqlClient;
using System.Data;

namespace GirlyShopBackend.Infrastructure.Persistence.Dappers;

// برای کوئری‌های گزارش‌گیری سنگین که EF Core بهینه نیست (مثلاً داشبورد فروش)
public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
