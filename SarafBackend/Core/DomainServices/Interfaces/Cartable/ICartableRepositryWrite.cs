using CommonLibrary.Core.Domain;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Cartable;

namespace LoanMonitoringMicroService.Core.DomainServices.Interfaces
{
    public interface ICartableRepositryWrite
    {
        Task<ResultObject<int>> SetCartable(UserCartableVM request);
        Task<ResultObject<int>> RemoveCartable(int loanID, int SurveyListTypeId);
        Task<ResultObject<int>> RemoveCartableByID(int id);
        Task<ResultObject<int>> RemoveCartableList(List<int> ids);
    }
}
