using CommonLibrary.Core.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Core.Domain.Entities
{
    [Table("UserSurveyWage", Schema = "survey")]
    public class UserSurveyWage : BaseVision
    {
        public int UserId { get; set; }
        public long WageAmount { get; set; }
        public int LongDistance { get; set; }
        public int Distance { get; set; }
        public int PointCount { get; set; }
        public int ShortDistancePointCount { get; set; }
        public DateTime ComputeDate { get; set; }
        public int UserSurveyWageStatusTypeId { get; set; }
        public UserSurveyWageStatusType UserSurveyWageStatusType { get; set; }
        public int? SurveyWagePayInfoId { get; set; }
        public SurveyWagePayInfo SurveyWagePayInfo { get; set; }
    }
}
