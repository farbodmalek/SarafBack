using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibrary.Core.Domain.Entities.PlanNo
{
    [Table("PlanNo", Schema = "survey")]
    public class PlanNo
    {
        public int Id { get; set; }
        public byte SuveryEconomicTypeId { get; set; }
        public string Name { get; set; }
        public byte? NewSurveyEconomicTypeId { get; set; }
        public bool Approved { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        //public ICollection<LoanPlanDto> LoanPlans { get; set; }
        //public LoanSurveyEconomicTypeDto LoanSurveyEconomicType { get; set; }
    }
}
