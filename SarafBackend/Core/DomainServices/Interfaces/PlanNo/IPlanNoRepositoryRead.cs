using CommonLibrary.Core.Domain;
using LoanMonitoringMicroService.Application.DTO.Pwa;
using LoanMonitoringMicroService.Applications.AppService.ServiceDto.Surveys;
using LoanMonitoringMicroService.Core.Domain.ViewModel.PlanNo;
using LoanMonitoringMicroService.Domains.Core.Supervision.Entities.Dao;


namespace LoanMonitoringMicroService.Core.DomainServices.PlanNo
{
    public interface IPlanNoRepositoryRead
    {
        Task<ResultList<PlanNoVM>> GetPlanNoList(PlanNoFilterDto request);

        Task<ResultList<PlanNoHistoryVM>> GetPlanNoHistoryRequestList(PlanNoHistoryFilterVM request);

        Task<ResultList<LoanPlanNoStatistics>> GetLoanPlanNoList(PlanNoFilterDto request);
    }
}
