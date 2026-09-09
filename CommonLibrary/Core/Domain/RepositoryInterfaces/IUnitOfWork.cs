using System;

namespace CommonLibrary.Core.Domain.RepositoryInterfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ResultObject<int> SaveChanges();
        Task<ResultObject<int>> SaveChangesAsync();
    }
}
