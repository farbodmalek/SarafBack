using GirlyShop.AuthService.Application.Commands.Auth;
using GirlyShop.AuthService.Application.DTO;
using GirlyShop.AuthService.Application.Query.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GirlyShop.AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string? ClientIp => HttpContext.Connection.RemoteIpAddress?.ToString();

    // POST /api/auth/register  →  ثبت‌نام + رمز هش‌شده در دیتابیس
    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        [FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RegisterCommand(request.FullName, request.PhoneNumber, request.Password, ClientIp),
            cancellationToken);
        return Ok(result);
    }

    // POST /api/auth/login
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new LoginCommand(request.PhoneNumber, request.Password, ClientIp),
            cancellationToken);
        return Ok(result);
    }

    // POST /api/auth/refresh  →  گرفتن کارت ورود تازه
    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<AuthResponseDto>> Refresh(
        [FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RefreshTokenCommand(request.RefreshToken, ClientIp),
            cancellationToken);
        return Ok(result);
    }

    // POST /api/auth/logout  →  باطل کردن کلید رزرو
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new LogoutCommand(request.RefreshToken), cancellationToken);
        return NoContent();
    }

    // GET /api/auth/me  →  اطلاعات کاربری که لاگین کرده (برای تست توکن هم عالی است)
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserInfoDto>> Me(CancellationToken cancellationToken)
    {
        var sub = User.FindFirst("sub")?.Value;
        if (!int.TryParse(sub, out var userId))
        {
            return Unauthorized();
        }

        var user = await _mediator.Send(new GetCurrentUserQuery(userId), cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }
}
