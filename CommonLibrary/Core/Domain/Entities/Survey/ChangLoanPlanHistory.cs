using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CommonLibrary.Domains.Core.Supervision.Entities;
using CommonLibrary.Infrastructure.Utils.Extensions;

namespace CommonLibrary.Core.Domain.Entities.Survey
{
    [Table("ChangLoanPlanStateHistory", Schema = "survey")]
    public class ChangLoanPlanStateHistory
    {
        [Key]
        public int ID { get; set; }
        public int ChangeLoanPlanStateId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedDateFa { get { return CreatedDate.ConvertNullableGregorianToPersianDate(); } }
        public int CreatedBy { get; set; }
        //public string LoanPlanLongitude { get; set; }
        //public string LoanPlanLatitude { get; set; }
        //public string SurveyLatitude { get; set; }
        //public string SurveyLongitude { get; set; }
    }
}
