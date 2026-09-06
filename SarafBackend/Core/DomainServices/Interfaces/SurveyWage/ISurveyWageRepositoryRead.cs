using CommonLibrary.Core.Domain;
using LoanMonitoringMicroService.Application.DTO;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Survey;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Wage;
using LoanMonitoringMicroService.Domains.Core.Supervision.Entities.Dao;
using LoanMonitoringMicroService.Domains.Supervision.Service;

namespace LoanMonitoringMicroService.Core.DomainServices.Interfaces
{
    public interface ISurveyWageRepositoryRead
    {
        Task<ResultList<SurveyWageVM>> ComputeSurveyUsersWage(SurveyWageFilterVM request);
        Task<ResultList<SurveyWageVM>> ComputeSurveyUserWage(SurveyWageFilterVM request);
        Task<ResultList<SurveyWageVM>> GetSurveyWageList(SurveyWageFilterVM request);
        Task<SurveyWageDocumentVM> GetSurveyWageInfoReport(int surveyWagePayInfoId);
    }
}
