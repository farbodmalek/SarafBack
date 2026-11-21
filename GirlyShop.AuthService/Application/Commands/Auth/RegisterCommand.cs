using GirlyShop.AuthService.Application.DTO;
using MediatR;

namespace GirlyShop.AuthService.Application.Commands.Auth;

public record RegisterCommand(string FullName, string PhoneNumber, string Password, string? IpAddress)
    : IRequest<AuthResponseDto>;
