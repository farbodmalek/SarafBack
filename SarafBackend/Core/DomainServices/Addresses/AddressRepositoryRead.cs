using Dapper;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.Dappers;

namespace ShopMicroservice.Core.DomainServices.Addresses
{
    /// <summary>سمت Read آدرس‌ها (Dapper).</summary>
    public class AddressRepositoryRead : IAddressRepositoryRead
    {
        private readonly DapperContext _context;

        public AddressRepositoryRead(DapperContext context)
        {
            _context = context;
        }

        private const string Columns =
            "Id, Title, RecipientName, PhoneNumber, City, PostalCode, Detail, IsDefault";

        public async Task<List<AddressDto>> GetListAsync(int userId, CancellationToken cancellationToken)
        {
            var sql = $"SELECT {Columns} FROM Addresses WHERE UserId = @UserId ORDER BY IsDefault DESC, Id;";
            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<AddressDto>(
                new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
            return rows.ToList();
        }

        public async Task<AddressDto?> GetByIdAsync(int userId, int addressId, CancellationToken cancellationToken)
        {
            var sql = $"SELECT {Columns} FROM Addresses WHERE UserId = @UserId AND Id = @AddressId;";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<AddressDto>(
                new CommandDefinition(sql, new { UserId = userId, AddressId = addressId },
                    cancellationToken: cancellationToken));
        }
    }
}
