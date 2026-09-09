using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.Loans
{
    [Table("LoanContracts", Schema = "Core")]
    public class LoanContract 
    {
        [Key]
        public int Id { get; set; }
        public int? LoanId { get; set; } //fk
        public int BranchCode { get; set; }
        /// <summary>
        /// کد نوع تسهیلات
        /// </summary>
        public int LoanMinorTypeId { get; set; }
        public int CustomerNumber { get; set; }
        public int LoanSerial { get; set; }
        /// <summary>
        /// شماره پرونده تسهیلات
        /// </summary>
        public string? LoanNumber { get; set; }
        /// <summary>
        /// شماره شناسه
        /// </summary>
        public string? IdentifierNumber { get; set; }
        /// <summary>
        /// تاریخ شروع قرارداد
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// تاریخ پایان قرارداد
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// تاریخ درخواست
        /// </summary>
        public DateTime RequestDate { get; set; }
        /// <summary>
        /// تاریخ تصویب
        /// </summary>
        public DateTime AcceptDate { get; set; }
        /// <summary>
        /// مبلغ مصوب
        /// </summary>
        public long AcceptAmount { get; set; }
        /// <summary>
        /// مبلغ درخواستی
        /// </summary>
        public long RequestAmount { get; set; }
        /// <summary>
        /// طبقه بندی منطقه
        /// </summary>
        public int? ZoneId { get; set; } // table
        public byte SystemTypeId { get; set; } // table 

        /// <summary>
        /// رسته پرونده
        /// </summary>
        public int? LoanClassTypeId { get; set; } // table
        /// <summary>
        /// بخش اقتصادی
        /// </summary>
        public int LoanEconomicTypeId { get; set; } // table

        /// <summary>
        /// رشته فعالیت
        /// </summary>
        public int? LoanActivityTypeId { get; set; } // table
        /// <summary>
        /// کد ISIC
        /// </summary>
        public int LoanISICCodeTypeId { get; set; } // table
        /// <summary>
        /// طول دوره اقساط
        /// </summary>                    
        public int ContractDuration { get; set; } // CREXPRT 

        //public LoanEconomicType LoanEconomicType { get; set; }
        //   public LoanActivityType LoanActivityType { get; set; }
        //public LoanISICCodeType LoanISICCodeType { get; set; }
        //  public LoanClassType LoanClassType { get; set; }
        public Loan? Loan { get; set; }
        /// <summary>
        /// نوع قسط - سالیانه - ماهیانه - فصلی
        /// </summary>
        public int? LoanInstallmentTypeId { get; set; } // table    
        [NotMapped]
        public string? LoanInstallmentTypeDesc { get; set; } // table    
        //public LoanInstallmentType LoanInstallmentType { get; set; } // table    
        public double? PenaltyRate { get; set; }
        public bool IsShowing { get; set; }
        public double? ProfitRate { get; set; }
        public double? ProfitPercent { get; set; }
    }
}
