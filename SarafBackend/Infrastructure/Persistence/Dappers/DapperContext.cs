using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ShopMicroservice.Infrastructure.Persistence.Dappers
{
    /// <summary>
    /// یک IDbConnection سبک برای Dapper می‌سازد. عمداً هیچ ORM/Tracking‌ای
    /// درگیر نیست تا سمت Read تا حد امکان سریع بماند.
    /// </summary>
    public class DapperContext
    {
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ShopReadConnection")
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not configured.");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
