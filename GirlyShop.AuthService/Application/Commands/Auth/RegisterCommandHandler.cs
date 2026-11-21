using GirlyShop.AuthService.Application.DTO;
using GirlyShop.AuthService.Core.Domain;
using GirlyShop.AuthService.Core.Exceptions;
using GirlyShop.AuthService.Core.Interfaces;
using MediatR;

namespace GirlyShop.AuthService.Application.Commands.Auth;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserRepositoryWrite _userWrite;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthTokenIssuer _tokenIssuer;

    public RegisterCommandHandler(
        IUserRepositoryWrite userWrite,
        IPasswordHasher passwordHasher,
        IAuthTokenIssuer tokenIssuer)
    {
        _userWrite = userWrite;
        _passwordHasher = passwordHasher;
        _tokenIssuer = tokenIssuer;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userWrite.PhoneExistsAsync(request.PhoneNumber, cancellationToken))
        {
            throw new ConflictException("این شماره موبایل قبلاً ثبت‌نام کرده است.");
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            PhoneNumber = request.PhoneNumber,

            // ⭐ نکته اصلی: رمز خام هیچ‌وقت ذخیره نمی‌شود، فقط نسخه هش‌شده
            PasswordHash = _passwordHasher.Hash(request.Password),

            IsAdmin = false,   // همه با نقش مشتری شروع می‌کنند
            IsActive = true
        };

        await _userWrite.AddAsync(user, cancellationToken);
        await _userWrite.SaveChangesAsync(cancellationToken);

        var info = new UserInfoDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Phone = user.PhoneNumber,
            Email = user.Email,
            IsAdmin = user.IsAdmin
        };

        return await _tokenIssuer.IssueAsync(info, request.IpAddress, cancellationToken);
    }
}
