using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Dto;
using LoanMonitoringMicroService.Application.DTO.Survey;
using LoanMonitoringMicroService.Core.Domain.Entities;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Survey;

namespace LoanMonitoringMicroService.Core.DomainServices.Interfaces
{
    public interface ISurveyListRepositoryRead
    {
        Task<ResultList<SurveyVM>> GetSurveysList(SurveyFilterVM request);
        Task<ResultList<SurveyVM>> GetReferenceToMeSurveyList(SurveyFilterVM request);
        Task<ResultList<SurveyReferenceBaseInfoVM>> GetSurveyReferenceBaseInfoList( string? KeyWord, List<int>? ids, int? surveyReferenceBaseInfoId);
        Task<ResultList<SurveyReferenceMaxCountVM>> GetSurveyMaxCountCondition(BaseFilter request);
 
        Task<ResultObject<int>> FillAllowedSupervisionLoan();
    }
}
