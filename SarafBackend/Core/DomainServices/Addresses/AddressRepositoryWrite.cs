using Microsoft.EntityFrameworkCore;
using ShopMicroservice.Core.Domain;
using ShopMicroservice.Core.Interfaces;
using ShopMicroservice.Infrastructure.Persistence.EntityFramework;

namespace ShopMicroservice.Core.DomainServices.Addresses
{
    /// <summary>سمت Write آدرس‌ها (EF Core).</summary>
    public class AddressRepositoryWrite : IAddressRepositoryWrite
    {
        private readonly ShopDbContext _db;

        public AddressRepositoryWrite(ShopDbContext db)
        {
            _db = db;
        }

        public Task<Address?> GetTrackedAsync(int userId, int addressId, CancellationToken cancellationToken)
            => _db.Addresses.FirstOrDefaultAsync(
                a => a.UserId == userId && a.Id == addressId, cancellationToken);

        public Task<int> CountAsync(int userId, CancellationToken cancellationToken)
            => _db.Addresses.CountAsync(a => a.UserId == userId, cancellationToken);

        public async Task AddAsync(Address address, CancellationToken cancellationToken)
            => await _db.Addresses.AddAsync(address, cancellationToken);

        public void Remove(Address address) => _db.Addresses.Remove(address);

        public async Task ClearDefaultAsync(int userId, int? exceptAddressId, CancellationToken cancellationToken)
        {
            var exceptId = exceptAddressId ?? 0;
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Addresses SET IsDefault = 0 WHERE UserId = {userId} AND IsDefault = 1 AND Id <> {exceptId}",
                cancellationToken);
        }

        public async Task PromoteOldestAsDefaultAsync(int userId, CancellationToken cancellationToken)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE Addresses SET IsDefault = 1
                   WHERE Id = (SELECT TOP 1 Id FROM Addresses WHERE UserId = {userId} ORDER BY CreatedAt, Id)",
                cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => _db.SaveChangesAsync(cancellationToken);
    }
}
