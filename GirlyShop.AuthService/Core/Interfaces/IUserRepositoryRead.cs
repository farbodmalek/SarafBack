using GirlyShop.AuthService.Application.DTO;

namespace GirlyShop.AuthService.Core.Interfaces;

// سمت Read — با Dapper
public interface IUserRepositoryRead
{
    Task<UserAuthDto?> GetByPhoneAsync(string phoneNumber, CancellationToken cancellationToken);
    Task<UserAuthDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
