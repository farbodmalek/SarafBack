using CommonLibrary.Core.Domain.Entities;
using CommonLibrary.Core.Domain.Entities.Attachment;
using CommonLibrary.Core.Domain.Entities.Customers;
using CommonLibrary.Core.Domain.Entities.Loans;
using CommonLibrary.Core.Domain.Entities.PlanNo;
using CommonLibrary.Core.Domain.Entities.Survey;
using CommonLibrary.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.Domain.Entities;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Survey;
using LoanMonitoringMicroService.Domains.Core.Supervision.Entities;
using LoanMonitoringMicroService.Domains.Supervision.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanMonitoringMicroService.Core.Domain.RepositoryInterfaces
{
    public interface ILoanMonitoringDbContext : IUnitOfWork
    {
        DbSet<Survey> Surveys { get; set; }
        DbSet<OutsideSurvey> OutsideSurvey { get; set; }

        //DbSet<LegalReferenceInfo> LegalReferenceInfos { get; set; }
        
        DbSet<LoanPlanNo> LoanPlanNo { get; set; }
        DbSet<LoanPlan> LoanPlans { get; set; }
        DbSet<PlanNo> PlanNos { get; set; }
        DbSet<PlanNoHistory> PlanNoHistories { get; set; }
        DbSet<Cartable> Cartables { get; set; }
        DbSet<Loan> Loans { get; set; }
        DbSet<LoanContract> LoanContracts { get; set; }
        DbSet<Branch> Branches { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<SurveyConfig> SurveyConfigs { get; set; }
        DbSet<LoanSurveyEconomicType> LoanSurveyEconomicTypes { get; set; }
        DbSet<PlanActivationType> PlanActivationTypes { get; set; }
        DbSet<ChangeLoanPlanState> ChangeLoanPlanStates { get; set; }
        DbSet<ChangLoanPlanStateHistory> ChangLoanPlanHistories { get; set; }
        DbSet<LoanActionLog> LoanActionLogs { get; set; }
        DbSet<LoanRedundantView> LoanRedundantViews { get; set; }

        DbSet<UserSurveyWage> UserSurveyWages { get; set; }
        DbSet<SurveyWagePayInfo> SurveyWagePayInfos { get; set; }
        DbSet<Attachments> Attachments { get; set; }
        DbSet<LoanGuaranter> LoanGuaranties { get; set; }
        DbSet<SurveyWageLoanPlan> SurveyWageLoanPlan { get; set; }

        DbSet<SurveyMaxCount> SurveyReferenceMaxCounts { get; set; }
        DbSet<SurveyReferenceReagent> SurveyReferenceReagents { get; set; }
        DbSet<SurveyReferenceContractDate> SurveyReferenceContractDates { get; set; }
        DbSet<SurveyReferenceLoanAmount> SurveyReferenceLoanAmounts { get; set; }
        DbSet<AllowedFirstTimeSupervisionRole> AllowedFirstTimeSupervisionRoles { get; set; }
        DbSet<SurveyReferenceBaseInfo> SurveyReferenceBaseInfos { get; set; }

        DbSet<PlanLivestockSurvey> PlanLivestockSurveys { get; set; }
        DbSet<PlanIndustrialSurvey> PlanIndustrialSurveys { get; set; }
        DbSet<PlanGardenSurvey> PlanGardenSurveys { get; set; }
        DbSet<PlanServiceSurvey> PlanServiceSurveys { get; set; }

        DbSet<LoanMinorType> LoanMinorTypes { get; set; }

    }
}
