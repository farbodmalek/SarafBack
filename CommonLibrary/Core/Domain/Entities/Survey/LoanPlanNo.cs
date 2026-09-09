using CommonLibrary.Core.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibrary.Core.Domain.Entities.Survey
{
    [Table("LoanPlanNo", Schema = "survey")]
    public class LoanPlanNo : BaseVision
    {
        public int LoanSurveyEconomicTypeId { get; set; }
        public int? PlanNoId { get; set; }
        //public PlanNo PlanNo { get; set; }
        public int LoanId { get; set; }
        public int? LoanSurveyEconomicTypeId1 { get; set; }
        public int? UserOtherPlanNo { get; set; }
        public string? UserPlanNoText { get; set; }
    }
}
