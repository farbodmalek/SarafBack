using MediatR;

namespace GirlyShop.AuthService.Application.Commands.Auth;

public record LogoutCommand(string RefreshToken) : IRequest;
