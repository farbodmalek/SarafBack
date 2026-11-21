using GirlyShop.AuthService.Application.DTO;
using GirlyShop.AuthService.Core.Exceptions;
using GirlyShop.AuthService.Core.Interfaces;
using MediatR;

namespace GirlyShop.AuthService.Application.Commands.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepositoryRead _userRead;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthTokenIssuer _tokenIssuer;

    public LoginCommandHandler(
        IUserRepositoryRead userRead,
        IPasswordHasher passwordHasher,
        IAuthTokenIssuer tokenIssuer)
    {
        _userRead = userRead;
        _passwordHasher = passwordHasher;
        _tokenIssuer = tokenIssuer;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRead.GetByPhoneAsync(request.PhoneNumber, cancellationToken);

        // عمداً یک پیام برای هر دو حالت «کاربر نیست» و «رمز اشتباه است»:
        // تا کسی نتواند بفهمد کدام شماره‌ها ثبت‌نام کرده‌اند
        if (user is null
            || !user.IsActive
            || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new AuthenticationFailedException("شماره موبایل یا رمز عبور اشتباه است.");
        }

        return await _tokenIssuer.IssueAsync(user.ToInfo(), request.IpAddress, cancellationToken);
    }
}
