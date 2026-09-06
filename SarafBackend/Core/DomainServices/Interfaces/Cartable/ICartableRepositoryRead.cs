using CommonLibrary.Core.Domain;

namespace LoanMonitoringMicroService.Core.DomainServices.Interfaces
{
    public interface ICartableRepositryRead
    {

        Task<ResultObject<int>> GetLastCartableByLoanId(int request);
    }
}
