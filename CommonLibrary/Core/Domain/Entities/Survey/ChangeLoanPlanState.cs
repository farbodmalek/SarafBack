using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CommonLibrary.Domains.Core.Supervision.Entities;

namespace CommonLibrary.Core.Domain.Entities.Survey
{
    [Table("ChangeLonaPlanState", Schema = "survey")]
    public class ChangeLoanPlanState
    {
        [Key]
        public int ID { get; set; }
        public int LoanPlanId { get; set; }
        public int LoanId { get; set; }
        public int RequestBy { get; set; }
        public DateTime RequestDate { get; set; }
        public int? VerifyBy { get; set; }
        public DateTime? VerifyDate { get; set; }
        public DateTime? ChangeDate { get; set; }
        public int? ChangeBy { get; set; }
        public int? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsConfirmed { get; set; }
        public string? Description { get; set; }



        public ICollection<LoanPlanLocationState> LoanPlanLocationStates { get; set; }
    }
}
