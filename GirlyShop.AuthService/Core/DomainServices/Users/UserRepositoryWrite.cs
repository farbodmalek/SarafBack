using GirlyShop.AuthService.Core.Domain;
using GirlyShop.AuthService.Core.Exceptions;
using GirlyShop.AuthService.Core.Interfaces;
using GirlyShop.AuthService.Infrastructure.Persistence.EntityFramework;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GirlyShop.AuthService.Core.DomainServices.Users;

// سمت Write: Entity Framework
public class UserRepositoryWrite : IUserRepositoryWrite
{
    private readonly AuthDbContext _context;

    public UserRepositoryWrite(AuthDbContext context)
    {
        _context = context;
    }

    public Task<bool> PhoneExistsAsync(string phoneNumber, CancellationToken cancellationToken)
        => _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken)
        => await _context.Users.AddAsync(user, cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        // اگه دو نفر دقیقاً هم‌زمان با یک شماره ثبت‌نام کنند، دیتابیس جلوی دومی را می‌گیرد
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new ConflictException("این شماره موبایل قبلاً ثبت‌نام کرده است.");
        }
    }
}
