using GirlyShop.AuthService.Application.DTO;
using MediatR;

namespace GirlyShop.AuthService.Application.Query.Auth;

public record GetCurrentUserQuery(int UserId) : IRequest<UserInfoDto?>;
