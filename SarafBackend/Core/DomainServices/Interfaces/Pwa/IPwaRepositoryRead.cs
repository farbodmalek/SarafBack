using CommonLibrary.Core.Domain;
using LoanMonitoringMicroService.Application.DTO.Pwa;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Pwa;

namespace LoanMonitoringMicroService.Core.DomainServices.Interfaces
{
    public interface IPwaRepositoryRead
    {

        Task<ResultList<SurveyCartableVM>> GetSurveysCartableList(SurveyCartableFilterDTO request);
    }
}
