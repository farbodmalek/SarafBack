using CommonLibrary.Core.Domain.RepositoryInterfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CommonLibrary.Infrastructure.Persistence.Dappers
{
    public class DapperHandler : IDapperHandler
    {
        public enum DBType
        {
            SQL,
            Oracle,
        }

        private readonly string _connectionString;
        private readonly DBType _dbType;

        public DapperHandler(string connectionString, DBType _dbType = DBType.SQL)
        {
            _connectionString = connectionString;
        }
        public IDbConnection CreateConnection()
        {
            if (this._dbType == DBType.Oracle)
            {
                //return new OleDbConnection(_connectionString);
            }
            return new SqlConnection(_connectionString);
        }

        public void Dispose()
        {

        }

        public DapperHandler Create(string connectionString)
        {
            return new DapperHandler(_connectionString);
        }

        public int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            throw new NotImplementedException();
        }

        public async Task<T> Get<T>(string sp, DynamicParameters parms = null, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = CreateConnection();
            return await db.QueryFirstOrDefaultAsync<T>(sp, parms, commandType: commandType);
        }

        public async Task<List<T>> GetAll<T>(string sp, DynamicParameters parms = null, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = CreateConnection();
            if (parms == null)
                return (await db.QueryAsync<T>(sp)).ToList();
            return (await db.QueryAsync<T>(sp, parms, commandType: commandType)).ToList();
        }
        public async Task<T> GetFirstOrDefault<T>(string sp, DynamicParameters parms = null, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = CreateConnection();
            if (parms == null)
                return (await db.QueryAsync<T>(sp)).FirstOrDefault();
            return (await db.QueryAsync<T>(sp, parms, commandType: commandType)).FirstOrDefault();
        }
        public async Task<T> ExecuteScalar<T>(string sp)
        {
            using IDbConnection db = CreateConnection();
            return await db.ExecuteScalarAsync<T>(sp);
        }
        public async Task<int> ExecuteAsync(string sp, DynamicParameters parms = null)
        {
            using IDbConnection db = CreateConnection();
            return await db.ExecuteAsync(sp, parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<T> GetSingleOrDefault<T>(string sp, DynamicParameters parms = null, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = CreateConnection();
            if (parms == null)
                return (await db.QueryAsync<T>(sp)).SingleOrDefault();
            return (await db.QueryAsync<T>(sp, parms, commandType: commandType)).SingleOrDefault();
        }

        public async Task<T> GetMultipleResults<T>(string query)
        {
            var querys = "SELECT * FROM Companies WHERE Id = @Id;" +
                        "SELECT * FROM Employees WHERE CompanyId = @Id";
            using (var connection = CreateConnection())
            using (var multi = await connection.QueryMultipleAsync(query))
            {
                var doc = await multi.ReadSingleOrDefaultAsync<T>();
                //if (doc != null)
                //    doc.Employees = (await multi.ReadAsync<Employee>()).ToList();
                return doc;
            }
        }
        public async Task<List<T>> GetMultipleMapping<T>()
        {
            var query = "SELECT * FROM Companies c JOIN Employees e ON c.Id = e.CompanyId";

            using (var connection = CreateConnection())
            {
                var dict = new Dictionary<int, T>();

                //var companies = await connection.QueryAsync<T, Employee, Company>(
                //    query, (company, employee) =>
                //    {
                //        if (!dict.TryGetValue(company.Id, out var currentCompany))
                //        {
                //            currentCompany = company;
                //            dict.Add(currentCompany.Id, currentCompany);
                //        }

                //        currentCompany.Employees.Add(employee);
                //        return currentCompany;
                //    }
                //);

                //return companies.Distinct().ToList();
                return new List<T>();
            }
        }
    }
}
