using GirlyShop.AuthService.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace GirlyShop.AuthService.Infrastructure.Persistence.EntityFramework;

// فقط دو جدول: Users و RefreshTokens
public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
