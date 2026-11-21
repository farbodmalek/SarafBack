using GirlyShop.AuthService.Application.DTO;
using GirlyShop.AuthService.Core.Interfaces;
using MediatR;

namespace GirlyShop.AuthService.Application.Query.Auth;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserInfoDto?>
{
    private readonly IUserRepositoryRead _userRead;

    public GetCurrentUserQueryHandler(IUserRepositoryRead userRead)
    {
        _userRead = userRead;
    }

    public async Task<UserInfoDto?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRead.GetByIdAsync(request.UserId, cancellationToken);
        return user is { IsActive: true } ? user.ToInfo() : null;
    }
}
