using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Core.Domain.Entities.Loans
{
    [Table("LoanRedundantView", Schema = "Core")]
    public class LoanRedundantView 
    {
        [Key]
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public int LoanId { get; set; }
        public int? SurveyCartableCount { get; set; }
        public int? SurveyLevel { get; set; }
        public int SurveyCount { get; set; }
        public int? NotScheduledReferredSurveyCount { get; set; }
        public int OutSideSurveyCount { get; set; }
        public DateTime? LastOutSideSurveyDate { get; set; }
        public DateTime? LastReferenceDate { get; set; }
        public DateTime? LastSurveyDate { get; set; }
        public int? LastSurveyUserId { get; set; }
        public int? HeaderLoanId { get; set; }
        public bool? IsHeaderSupervised { get; set; }
        public int? LastCartableId { get; set; }
        public int? LastExistCartableId { get; set; }
        public DateTime? LastChangeStatusDate { get; set; }
        public long? WaitProfitAmountInfo { get; set; }
        public long? LastBankPaymentAmount { get; set; }
        public DateTime? LastPayCardDate { get; set; }
        public bool ReferenceToLegal { get; set; }
        public bool IsNotShowInCentralBank { get; set; }
        public long? LoanPayCardAmount { get; set; }
        public long? LoanPayCardCurrentAmount { get; set; }

    }
}
