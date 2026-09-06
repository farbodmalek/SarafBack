using CommonLibrary.Core.Domain.Entities.PlanNo;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Core.Domain.Entities
{
    [Table("SurveyWageLoanPlan", Schema = "survey")]
    public class SurveyWageLoanPlan 
    {
        [Key]
        public int Id { get; set; }
        public int UserSurveyWageId { get; set; }
        public int LoanPlanId { get; set; }
        public UserSurveyWage UserSurveyWage { get; set; }
        public LoanPlan LoanPlan { get; set; }
    }
}
