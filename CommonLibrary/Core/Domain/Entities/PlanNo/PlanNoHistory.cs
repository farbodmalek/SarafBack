using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.PlanNo
{
    [Table("PlanNoHistory", Schema = "survey")]
    public class PlanNoHistory 
    {
        [Key]
        public int Id { get; set; }
        public int LoanPlanId { get; set; }
        public LoanPlan LoanPlan { get; set; }
        public int LoanId { get; set; }
        public Loan Loan { get; set; }
        public int PlanNoId { get; set; }
        //public PlanNo PlanNo { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public int RequestUserId { get; set; }
        public int? ConfirmUserId { get; set; }
        public int? PlanNoStatusId { get; set; }
        public int? CartableId { get; set; }
        public string? OtherPlanNo { get; set; }
        public string? Description { get; set; }
    }
}
