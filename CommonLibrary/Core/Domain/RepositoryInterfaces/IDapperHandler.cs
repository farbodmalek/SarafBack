using Dapper;
using System.Data;

namespace CommonLibrary.Core.Domain.RepositoryInterfaces
{
    public interface IDapperHandler : IDisposable
    {
        Task<T> Get<T>(string sp, DynamicParameters parms = null, CommandType commandType = CommandType.Text);
        Task<List<T>> GetAll<T>(string sp, DynamicParameters parms = null, CommandType commandType = CommandType.Text);
        Task<T> GetFirstOrDefault<T>(string sp, DynamicParameters parms = null, CommandType commandType = CommandType.Text);
        Task<T> GetSingleOrDefault<T>(string sp, DynamicParameters parms = null, CommandType commandType = CommandType.Text);
        Task<T> ExecuteScalar<T>(string sp);
        Task<int> ExecuteAsync(string sp, DynamicParameters param);

    }
}
