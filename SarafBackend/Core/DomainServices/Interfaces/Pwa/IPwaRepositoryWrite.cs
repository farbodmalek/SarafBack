using CommonLibrary.Core.Domain;
using LoanMonitoringMicroService.Application.DTO.Pwa;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Cartable;

namespace LoanMonitoringMicroService.Core.DomainServices.Interfaces
{
    public interface IPwaRepositoryWrite
    {
        Task<ResultObject<int>> SetLoanPlanSurvey(PwaSurveyLoanPlanVM request);

    }
}
