using System.Data;
using Microsoft.Data.SqlClient;

namespace GirlyShop.AuthService.Infrastructure.Persistence.Dappers;

public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection تنظیم نشده است.");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
