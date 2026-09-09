using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.Survey
{
    [Table("SurveyConfig", Schema = "survey")]
    public class SurveyConfig
    {
        [Key]
        public int Id { get; set; }
        public int MaxDistance { get; set; }                 // حداکثر فاصله
        public int SurveyExpireDeadline { get; set; }        // مهلت نظارت
        public int PreaperSurveyDelay { get; set; }          // مدت زمان انتظار جهت برگشت به لیست جهت ارجاع
        public long MaxAllowedLoanAmount { get; set; }       // حداکثر مبلغ مجاز جهت ارجاع به نیرو حق السعی
        public long HouseholdLoanAmount { get; set; }        // مینیم مقدار خانگی
        public long SelfExpressionLoanAmount { get; set; }   //مینیم مقدار خویش فرمایی
        public long WageAmountPerLoan { get; set; }      // مبلغ حق السعی برای هر پرونده
        public long CityDistanceWageAmount { get; set; } // مبلغ بر اساس مسافت
        public long DistanceDimension { get; set; } // بعد مصافت
        public int PaymentMonthDeadline { get; set; } // مهلت برای ماه پرداخت
    }
}
