using CommonLibrary.Core.Domain.Entities.Loans;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.Common
{
    public class BaseFact : BaseLoan
    {
        [Key]
        public int Id { get; set; }
        public int? LoanId { get; set; }

        public DateTime EffectiveDate { get; set; }
        /// <summary>
        /// تعداد اقساط معوق
        /// </summary>
        public int NumberOfDelayedInstallment { get; set; }
        /// <summary>
        /// تاریخ آخرین قسط پرداخت شده
        /// </summary>
        public DateTime? LastInstallmentDate { get; set; }
        /// <summary>
        /// سررسید اولین قسط
        /// </summary>
        public DateTime? FirstInstallmentDate { get; set; }
        /// <summary>
        /// مبلغ هر قسط
        /// </summary>
        public long? EachInstallmentAmount { get; set; }
        /// <summary>
        /// کل مبلغ معوق 
        /// </summary>
        public long? TotalDelayedAmount { get; set; }
        /// <summary>
        /// کل بازپرداختی 
        /// </summary>
        public long? TotalRefundAmount { get; set; }

        /// <summary>
        /// مورد انتظار وصول
        /// </summary>
        public long? VisionAmount { get; set; }


        /// <summary>
        /// مبلغ تسهلات پرداخت شده
        /// </summary>
        public long? BankPayAmount { get; set; }

        /// <summary>
        /// مانده تعهدات
        /// </summary>
        public long? RemainAmount { get; set; }

        /// <summary>
        /// مانده از اصل
        /// </summary>
        public long? RemainFromAmount { get; set; }
        /// <summary>
        /// مانده از کارمزد
        /// </summary>
        public long? RemainFromProfit { get; set; }

        /// <summary>
        /// جریمه تاخیر
        /// در حال حاضر قابل محاسبه نیست
        /// </summary>
        public long? DelayedAmount { get; set; }

        /// <summary>
        /// تاریخ اولین قسط پرداخت نشده
        /// </summary>
        public DateTime? FirstDelayedInstallmentDate { get; set; }

        /// <summary>
        /// تاریخ آخرین قسط پرداخت نشده
        /// </summary>

        public DateTime? LastDelayedInstallmentDate { get; set; }



        //---تا اینجا
        /// <summary>
        /// کل مطالبات اصل و کارمزد        
        /// </summary>
        public long? TotalRemainAmount { get; set; }
        /// <summary>
        /// کل بدهی(اصل + کارمزد + جریمه)
        /// در حال حاضر قابل محاسبه نیست
        /// </summary>
        public long? TotalRemainAmountWithPenalty { get; set; }




        /// <summary>
        /// وضعیت وام
        /// </summary>
        /// 
        [Column(TypeName = "char(1)")]
        public string LoanStatusTypeId { get; set; } // TABLE

        /// <summary>
        ///  تعداد اقساط سر رسید شده پرداخت نشده
        /// </summary>
        /// تعداد اقساط معوق
        public int MaturedInstallmentsCount { get; set; }


        /// <summary>
        ///  تعداد اقساط پرداخت شده
        /// </summary>
        /// 
        public int PayedInstallmentsCount { get; set; }


        /// <summary>
        ///  تعداد اقساط  باقیمانده
        /// </summary>
        /// 
        public int TotalInstallmentsCount { get; set; }

        /// <summary>
        ///  بازپرداختی از اصل
        /// </summary>
        /// 
        public long? PayedAmount { get; set; }

        /// <summary>
        ///  بازپرداختی از کارمزد
        /// </summary>
        public long? PayedProfit { get; set; }

        /// <summary>
        ///  بازپرداختی از جریمه
        /// </summary>
        public long PayedFromPenalty { get; set; }

        /// <summary>
        ///  تاریخ آخرین پرداخت بانک
        /// </summary>
        public DateTime LastBankPay { get; set; }

        /// <summary>
        /// تعداد پرداخت صندوق
        /// </summary>
        public long? BankPayCount { get; set; }




        /// <summary>
        /// بدهی معوق
        /// </summary>
        public long? DelayedDebtAmount { get; set; }
        /// <summary>
        /// معوق از اصل
        /// </summary>
        public long? DelayedFromAmount { get; set; }

        /// <summary>
        ///  معوق از کارمزد
        /// </summary>
        public long? DelayedFromProfit { get; set; }



        /// <summary>
        /// مبلغ تخفیف
        /// </summary>
        /// قابل محاسبه نیست
        public long? DiscountAmount { get; set; }

        public long? LoanAmount { get; set; }

        public LoanStatusType LoanStatusType { get; set; }
        /// <summary>
        ///  جریمه
        /// </summary>
        /// قابل محاسبه نیست
        public long? PenaltyAmount { get; set; }
        /// <summary>
        /// تعداد اقساط
        /// </summary>
        public int InstallmentCount { get; set; }
        /// <summary>
        /// پرداختی قبل از سررسید
        /// </summary>
        /// 


        public long? BeforeOfMatureAmount { get; set; }
        /// <summary>
        /// پرداختی در سررسید
        /// </summary>
        public long? MatureAmount { get; set; }
        /// <summary>
        /// پرداختی بعد از سررسید
        /// </summary>
        public long? AfterOfMatureAmount { get; set; }

        /////// تا اینجا آمدیم


        //تا اینجا انجام گردیده است

        /// <summary>
        /// پرداختی در سررسید گذشته
        /// </summary>
        public long? PastOfMatureAmount { get; set; }

        /// <summary>
        /// پرداختی مشکوک الوصول
        /// </summary>
        public long? DoubtfulAmount { get; set; }
        /// <summary>
        /// پرداختی سود معوق
        /// </summary>
        public long? DelayedBenefitAmount { get; set; }

        //---مطالبات از اصل
        public long? DemandFromMain { get; set; }

        //---مطالبات از کارمزد
        public long? DemandFromProfit { get; set; }

        public DateTime? LastPayDate { get; set; }


        //---مانده تعهد از کارمزد معطلی
        public long? TotalRemainWaitProfitAmount { get; set; }

        //---مطالبات از کارمزد معطلی
        public long? RemainWaitProfitAmount { get; set; }


        //----کل سود معطلی
        public long? TotalPaidWaitProfitAmount { get; set; }

        public DateTime? LastAcceptDate { get; set; }
        public DateTime? LastPaymentInfoDate { get; set; }
        public long? WaitProfitAmount { get; set; }
        public long? LastBankPaymentAmount { get; set; }

        public long? RemainFromAmountAfterCurrent { get; set; }
        public long? RemainFromProfitAfterCurrent { get; set; }

    }
}
