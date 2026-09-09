using CommonLibrary.Core.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Core.Domain.Entities.Visions
{
    [Table("ReferenceHistory", Schema = "Vision")]
    public class ReferenceHistory : BaseFact
    {
        public int? ActorId { get; set; }
        [ForeignKey(nameof(ActorId))]   
        public LoanActor? Actor { get; set; }
        public int? CartableId { get; set; }
        public bool IsReference { get; set; }
        public int ReferenceTypeId { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public DateTime? ReferenceTime { get; set; }
        public DateTime? EndTime { get; set; }
        public static ReferenceHistory MakeEntity(BaseFact baseFact)
        {
            return new ReferenceHistory
            {
                Id = 0,
                LoanId = baseFact.LoanId,
                LoanNumber = baseFact.LoanNumber,
                DemandFromMain = baseFact.DemandFromMain,
                EffectiveDate = baseFact.EffectiveDate,
                NumberOfDelayedInstallment = baseFact.NumberOfDelayedInstallment,
                LastInstallmentDate = baseFact.LastInstallmentDate,
                FirstInstallmentDate = baseFact.FirstInstallmentDate,
                EachInstallmentAmount = baseFact.EachInstallmentAmount,
                TotalDelayedAmount = baseFact.TotalDelayedAmount,
                TotalRefundAmount = baseFact.TotalRefundAmount,
                VisionAmount = baseFact.VisionAmount,
                BankPayAmount = baseFact.BankPayAmount,
                RemainAmount = baseFact.RemainAmount,
                RemainFromAmount = baseFact.RemainFromAmount,
                RemainFromProfit = baseFact.RemainFromProfit,
                DelayedAmount = baseFact.DelayedAmount,
                FirstDelayedInstallmentDate = baseFact.FirstDelayedInstallmentDate,
                LastDelayedInstallmentDate = baseFact.LastDelayedInstallmentDate,
                TotalRemainAmount = baseFact.TotalRemainAmount,
                TotalRemainAmountWithPenalty = baseFact.TotalRemainAmountWithPenalty,
                LoanStatusTypeId = baseFact.LoanStatusTypeId,
                MaturedInstallmentsCount = baseFact.MaturedInstallmentsCount,
                PayedInstallmentsCount = baseFact.PayedInstallmentsCount,
                TotalInstallmentsCount = baseFact.TotalInstallmentsCount,
                PayedAmount = baseFact.PayedAmount,
                PayedProfit = baseFact.PayedProfit,
                PayedFromPenalty = baseFact.PayedFromPenalty,
                LastBankPay = baseFact.LastBankPay,
                BankPayCount = baseFact.BankPayCount,
                DelayedDebtAmount = baseFact.DelayedDebtAmount,
                DelayedFromAmount = baseFact.DelayedFromAmount,
                DelayedFromProfit = baseFact.DelayedFromProfit,
                DiscountAmount = baseFact.DiscountAmount,
                LoanAmount = baseFact.LoanAmount,
                PenaltyAmount = baseFact.PenaltyAmount,
                InstallmentCount = baseFact.InstallmentCount,
                BeforeOfMatureAmount = baseFact.BeforeOfMatureAmount,
                MatureAmount = baseFact.MatureAmount,
                AfterOfMatureAmount = baseFact.AfterOfMatureAmount,
                PastOfMatureAmount = baseFact.PastOfMatureAmount,
                DoubtfulAmount = baseFact.DoubtfulAmount,
                DelayedBenefitAmount = baseFact.DelayedBenefitAmount,
                DemandFromProfit = baseFact.DemandFromProfit,
                LastPayDate = baseFact.LastPayDate,
                TotalRemainWaitProfitAmount = baseFact.TotalRemainWaitProfitAmount,
                RemainWaitProfitAmount = baseFact.RemainWaitProfitAmount,
                TotalPaidWaitProfitAmount = baseFact.TotalPaidWaitProfitAmount,
                CreateDate = DateTime.Now,
            };
        }
    }
}
