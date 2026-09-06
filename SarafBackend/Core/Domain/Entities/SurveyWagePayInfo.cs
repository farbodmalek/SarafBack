using CommonLibrary.Core.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Core.Domain.Entities
{
    [Table("SurveyWagePayInfo", Schema = "survey")]
    public class SurveyWagePayInfo : BaseVision
    {
        public DateTime PaymentDate { get; set; }
        public int PaymentMonth { get; set; }
        public int PaymentYear { get; set; }
        public long WageAmount { get; set; }
        public int UserId { get; set; }
        public int LongDistance { get; set; }
        public int Distance { get; set; }
        public int PointCount { get; set; }
        public int ShortDistancePointCount { get; set; }
        public ICollection<UserSurveyWage> UserSurveyWages { get; set; }
    }
}
