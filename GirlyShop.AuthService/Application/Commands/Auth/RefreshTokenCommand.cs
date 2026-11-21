using GirlyShop.AuthService.Application.DTO;
using MediatR;

namespace GirlyShop.AuthService.Application.Commands.Auth;

public record RefreshTokenCommand(string RefreshToken, string? IpAddress) : IRequest<AuthResponseDto>;
