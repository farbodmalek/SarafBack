using CommonLibrary.Core.Domain;
using LoanMicroService.Application.DTO.Loans;
using LoanMonitoringMicroService.Applications.AppService.ServiceDto.Surveys;
using LoanMonitoringMicroService.Core.Domain.ViewModel.PlanNo;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Survey;

namespace LoanMonitoringMicroService.Core.DomainServices.Interfaces
{
    public interface ISurveyRepositoryRead
    {
        Task<ResultList<OutsideSurveyVM>> GetOutsideSurveyList(OutSideSurveyFilterDTO request);
        Task<ResultList<CartableSurveyVM>> GetCartableSurveyList(BaseSurveyFilterDTO request);        
        Task<ResultObject<SurveyDetailVM>> GetSurvyDetailById(BaseSurveyFilterDTO request);
        Task<ResultList<LoanPlanMarkerVM>> GetPlanMarkerList(LoanPlanMarkerFilterVM request);
        Task<ResultObject<LoanSurveyDocumentVM>> GetSurveyDocument(int loanId, int CartableId = 0, int? type=null);
        Task<ResultList<SurveyAddressRequestsVM>> GetSurveyAddressRequestsList(SurveyAddressRequestsFilterVM request);
        Task<ResultObject<SurveyAddressRequestsVM>> GetAddressRequestsById(int Id);
        Task<ResultObject<AddressRejectHistoryVM>> RejectAddressHistoryById(int Id);
    }
}
