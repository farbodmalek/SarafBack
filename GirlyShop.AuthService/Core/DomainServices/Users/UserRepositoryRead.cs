using Dapper;
using GirlyShop.AuthService.Application.DTO;
using GirlyShop.AuthService.Core.Interfaces;
using GirlyShop.AuthService.Infrastructure.Persistence.Dappers;

namespace GirlyShop.AuthService.Core.DomainServices.Users;

// سمت Read: SQL خام با Dapper (سریع)
public class UserRepositoryRead : IUserRepositoryRead
{
    private const string SelectColumns =
        "SELECT Id, FullName, PhoneNumber, Email, PasswordHash, IsAdmin, IsActive FROM Users";

    private readonly DapperContext _context;

    public UserRepositoryRead(DapperContext context)
    {
        _context = context;
    }

    public async Task<UserAuthDto?> GetByPhoneAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var command = new CommandDefinition(
            SelectColumns + " WHERE PhoneNumber = @PhoneNumber;",
            new { PhoneNumber = phoneNumber },
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<UserAuthDto>(command);
    }

    public async Task<UserAuthDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var command = new CommandDefinition(
            SelectColumns + " WHERE Id = @Id;",
            new { Id = id },
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<UserAuthDto>(command);
    }
}
