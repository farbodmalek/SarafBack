using CommonLibrary.Core.Domain.Entities;
using CommonLibrary.Core.Domain.Entities.Attachment;
using CommonLibrary.Core.Domain.Entities.Customers;
using CommonLibrary.Core.Domain.Entities.Loans;
using CommonLibrary.Core.Domain.Entities.PlanNo;
using CommonLibrary.Core.Domain.Entities.Survey;
using CommonLibrary.Infrastructure.Persistence.EntityFramework;
using LoanMonitoringMicroService.Core.Domain.Entities;
using LoanMonitoringMicroService.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Survey;
using LoanMonitoringMicroService.Domains.Core.Supervision.Entities;
using LoanMonitoringMicroService.Domains.Supervision.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LoanMonitoringMicroService.Infrastructure.Persistence.EntityFramework
{
    public class LoanMonitoringDbContext : BaseDbContext, ILoanMonitoringDbContext
    {
        public LoanMonitoringDbContext(DbContextOptions<BaseDbContext> options) : base(options)
        { }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<OutsideSurvey> OutsideSurvey { get; set; }
        
        public DbSet<LoanPlanNo> LoanPlanNo { get; set; }
        public DbSet<LoanPlan> LoanPlans { get; set; }
        public DbSet<PlanNo> PlanNos { get; set; }
        public DbSet<PlanNoHistory> PlanNoHistories { get; set; }
        public DbSet<Cartable> Cartables { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanContract> LoanContracts { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SurveyConfig> SurveyConfigs { get; set; }
        public DbSet<LoanSurveyEconomicType> LoanSurveyEconomicTypes { get; set; }
        public DbSet<PlanActivationType> PlanActivationTypes { get; set; }
        public DbSet<ChangeLoanPlanState> ChangeLoanPlanStates { get; set; }
        public DbSet<LoanActionLog> LoanActionLogs { get; set; }
        public DbSet<LoanRedundantView> LoanRedundantViews { get; set; }
        public DbSet<UserSurveyWage> UserSurveyWages { get; set; }
        public DbSet<SurveyWagePayInfo> SurveyWagePayInfos { get; set; }
        public DbSet<Attachments> Attachments { get; set; }
        public DbSet<LoanGuaranter> LoanGuaranties { get; set; }
        public DbSet<SurveyWageLoanPlan> SurveyWageLoanPlan { get; set; }
        public DbSet<SurveyMaxCount> SurveyReferenceMaxCounts { get; set; }
        public DbSet<SurveyReferenceReagent> SurveyReferenceReagents { get; set; }
        public DbSet<SurveyReferenceContractDate> SurveyReferenceContractDates { get; set; }
        public DbSet<SurveyReferenceLoanAmount> SurveyReferenceLoanAmounts { get; set; }
        public DbSet<AllowedFirstTimeSupervisionRole> AllowedFirstTimeSupervisionRoles { get; set; }
        public DbSet<SurveyReferenceBaseInfo> SurveyReferenceBaseInfos { get; set; }
        public DbSet<ChangLoanPlanStateHistory> ChangLoanPlanHistories { get; set; }
        public DbSet<PlanLivestockSurvey> PlanLivestockSurveys { get; set; }
        public DbSet<PlanIndustrialSurvey> PlanIndustrialSurveys { get; set; }
        public DbSet<PlanGardenSurvey> PlanGardenSurveys { get; set; }
        public DbSet<PlanServiceSurvey> PlanServiceSurveys { get; set; }
        public DbSet<LoanMinorType> LoanMinorTypes { get; set; }
        //public DbSet<LegalReferenceInfo> LegalReferenceInfos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<OutsideSurvey>()
            //    .ToTable("OutsideSurvey", schema: "survey");

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LoanPlan>()
       .ToTable(tb => tb.HasTrigger("updateLoanPlanGeoLoacation"))
       .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None)
       .HasAnnotation("SqlServer:UseOutputClause", false);
        }
    }
}
