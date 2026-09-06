using CommonLibrary.Core.Domain;
using LoanMonitoringMicroService.Application.DTO.Survey;
using LoanMonitoringMicroService.Core.Domain.Entities;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Survey;

namespace LoanMonitoringMicroService.Core.DomainServices.Interfaces
{
    public interface ISurveyRepositoryWrite
    {
        Task<ResultObject<int>> SetOutsideSurvey(OutsideSurveyVM request);
        Task<ResultObject<int>> ConfirmWageMontlyPayment(List<int> UserSurveyWageIds);
        //Task<ResultObject<int>> setOutsideSurvey(OutsideSurveyVM request);
        Task<ResultObject<int>> SetSurveyLoanAmountCondition(SurveyReferenceLoanAmountVM request);
        Task<ResultObject<int>> SetSurveyReferenceContractDateCondition(SurveyReferenceContractDateVM request);
        Task<ResultObject<int>> SetSurveyReferenceReagentCondition(SurveyReferenceReagentVM request);
        Task<ResultObject<int>> SetAllowedFirstTimeSupervisionRole(AllowedFirstTimeSupervisionRoleVM request);
        Task<ResultObject<int>> RemoveSurveyReferenceReagentCondition(int id);
        Task<ResultObject<int>> RemoveSurveyLoanAmountCondition(int id);
        Task<ResultObject<int>> RemoveAllowedFirstTimeSupervisionRole(int id);
        Task<ResultObject<int>> RemoveSurveyReferenceContractDateCondition(int id);
        Task<ResultObject<int>> SetSurveyBaseInfo(LoanMinorTypeVM loanMinorTypes);
        Task<ResultObject<int>> RemoveSurveyBaseInfo(int Id);
        Task<ResultObject<int>> ActiveSurveyBaseInfo(int Id, bool Active);
        Task<ResultObject<int>> SetSurveyMaxCountCondition(SurveyReferenceMaxCountVM request);
        Task<ResultObject<int>> RemoveSurveyMaxCountCondition(int Id);
        Task<ResultObject<int>> ConfirmSurveyAddressRequest(int Id, int SurveyId);
        Task<ResultObject<int>> RejectSurveyAddressRequest(int Id,string Desc);
    }
}
