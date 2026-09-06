using CommonLibrary.Core.Domain;
using LoanMonitoringMicroService.Application.DTO.Pwa;
using LoanMonitoringMicroService.Core.Domain.ViewModel.PlanNo;
using LoanMonitoringMicroService.Domains.Core.Supervision.Entities.Dao;

namespace LoanMonitoringMicroService.Core.DomainServices.PlanNo
{
    public interface  IPlanNoRepositoryWrite
    {
        Task<ResultList<PlanNoVM>> UpdateApprovedStatus(PlanNoFilterDto request);
        Task<ResultObject<int>> SetLoanPlanNo(LoanPlanNoVM request);
        Task<ResultList<PlanNoVM>> SetOtherPlanNo(PlanNoFilterDto request);
        Task<ResultObject<int>> RemovePlanNo(int Id);
        Task<ResultObject<int>> SetPlanNo(string name);
        Task<ResultObject<int>> ChangeLoansPlanNo(int CurrentPlanNoId, int NewPlanNoId);
        Task<ResultObject<int>> ChangePlanNoForLoans(int id, bool isConfrim, int planNoId, string? description);
    }
}
