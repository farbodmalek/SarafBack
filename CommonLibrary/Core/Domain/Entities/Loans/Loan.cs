using CommonLibrary.Core.Domain.Entities.Loans;
using CommonLibrary.Core.Domain.Entities.Survey;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CommonLibrary.Core.Domain.Entities.Customers;

namespace CommonLibrary.Core.Domain.Entities
{
    [Table("Loans", Schema = "Core")]
    public class Loan
    {
        public int Id { get; set; }
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
        public int? SharedLoanId { get; set; } //fk
        /// <summary>
        /// شماره بایگانی
        /// </summary>
        public int? ArchiveNumber { get; set; }
        public int? CustomerId { get; set; } // fk
        [MaxLength(600)]
        public string? LoanTitle { get; set; }
        /// <summary>
        /// دوره تنفس
        /// </summary>
        public int WaitDuration { get; set; }
        /// <summary>
        /// مبلغ هر قسط
        /// </summary>
        public long? EachInstallmentAmount { get; set; }
        /// <summary>
        /// سررسید اولین قسط
        /// </summary>
        public DateTime? FirstInstallmentDate { get; set; }
        /// <summary>
        /// تاریخ آخرین قسط
        /// </summary>
        public DateTime? LastInstallmentDate { get; set; }
        public DateTime RegisterDate { get; set; }
        //public ICollection<LoanContract> LoanContract { get; set; }
        public LoanContract? LoanContract { get; set; }
        //public LoanRedundantView LoanRedundantView { get; set; }
        //public ICollection<CustomerPlan> CustomerPlans { get; set; }
        //public ICollection<LoanCollat> LoanCollats { get; set; }
        //public ICollection<LoanGuarantee> LoanGuarantees { get; set; }
        //public ICollection<LoanPayCard> LoanPayCards { get; set; }
        //public ICollection<LoanPayInfo> LoanPayInfos { get; set; }
        //public ICollection<LoanPayBank> LoanPayBanks { get; set; }
        //public ICollection<PlanNoHistory> PlanNoHistories { get; set; }
        //public Cartable Cartable { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }
        //public FactLoanAcc FactLoanAcc { get; set; }
        //public ICollection<LoanPayDetail> LoanPayDetails { get; set; }
        //     public ICollection<LoanPlan> LoanPlans { get; set; }
        //   [NotMapped]
        //     public ICollection<Attachment> Attachments { get; set; }
        //public ICollection<Actor> Actors { get; set; }
        [NotMapped]
        public string? FacilitiesPart { get; set; }
        //public Branch Branche { get; set; }
        /// <summary>
        /// وضعیت وام
        /// </summary>
        /// 
        [Column(TypeName = "char(1)")]
        public string? LoanStatusTypeId { get; set; } // TABLE

        //public LoanStatusType LoanStatusType { get; set; }
        //public LoanMinorType LoanMinorType { get; set; }
        public int? ExceptionType { get; set; }

        public bool IsLegal { get; set; }
        public bool? IsReadyToLegal { get; set; }
        public bool? NotSupervision { get; set; }
        public int? LastActorId { get; set; }
        public int? LastActorUserId { get; set; }

        public int? LastRoleId { get; set; }

        //public int? LastRoleUserId { get; set; }

        public int? LastActionId { get; set; }
        public int? LastActionUserId { get; set; }
        public int? ActorStatusTypeId { get; set; }

        public int? LastActionTypeId { get; set; }
        public ICollection<Cartable>? Cartables { get; set; }
        public int DocumentTypeIdCount { get; set; }
        public DateTime? LastPaymentInfoDate { get; set; }
        public long? LastPaymentInfoAmount { get; set; }
        public DateTime? LastActorsDate { get; set; }

        public DateTime? LastPayBankDate { get; set; }

        //---تاریخ حقوقی
        public DateTime? LegalDate { get; set; }

        //---طرح برتر
        public bool IsPreferencePlan { get; set; }


        //---تاریخ طرح برتر
        public DateTime? PreferencePlanDate { get; set; }

        //---ماهیت تسهیلات
        public int? FacilitiesEssence { get; set; }


        //--محل اجرای طرح
        public int? LocationPlan { get; set; }

        //---عنوان کسب و کار
        public int? MarketingType { get; set; }
        public bool IsNotShowMarketingType { get; set; }
        [MaxLength(300)]
        public string? MarketingTypeDescCustomize { get; set; }
        public int? IsicEconomicTypeId { get; set; }
        public int? HeaderLoanId { get; set; }
        public int? ActorsCount { get; set; }
        public bool IsShowing { get; set; }
        public bool SurveyedInOldSystem { get; set; }
        public int LegalReferenceTypeId { get; set; }
        public int LegalUserId { get; set; }
        public int SurveyLevel { get; set; }
        public DateTime? LastSurveyDate { get; set; }
        //public ICollection<OutsideSurvey> OutsideSurveys { get; set; }
        //public ICollection<LegalReferenceInfo> LegalReferenceInfos { get; set; }
        // public int? NewLoanStatusTypeId { get; set; }
    }
}
