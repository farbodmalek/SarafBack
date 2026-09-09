using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.Visions
{
    [Table("Actors", Schema = "Vision")]
    public class Actor
    {
        [Key]
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? LoanId { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public DateTime? RollbackDate { get; set; }
        public string? Description { get; set; }
        public bool HasLoan { get; set; }
        public DateTime? ExpDate { get; set; }
        public int? ReferenceUserId { get; set; }
        public int? RollbackUserId { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? IsDeleted { get; set; }
        public long? Amount { get; set; }
        public string? BankDescription { get; set; }
        public DateTime? BankReceiptDate { get; set; }
        public string? BankReceiptNumber { get; set; }
        public bool? HasReceiptedAmount { get; set; }
        public long? WageAmount { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public int? ConfirmationUserId { get; set; }
        public int? WageId { get; set; }
        public int? ActorStatusTypeId { get; set; }
        public long? PreWageAmount { get; set; }
        public string? WageDescription { get; set; }
        public DateTime? ComputeDate { get; set; }
        public string? PreWageDescription { get; set; }
        public int? PaymentMonth { get; set; }
        public int? PrintRequestHistoryId { get; set; }
        public DateTime? ActorConfirmationDate { get; set; }
        public DateTime? CityConfirmationDate { get; set; }
        public int? CityConfirmationUserId { get; set; }
        public DateTime? SupervisorConfirmationDate { get; set; }
        public int? SupervisorConfirmationUserId { get; set; }
        public int? PrintRequestHistoryIdOld { get; set; }
        public long? PayedAmount { get; set; }
        public long? PayedFromDemandAmount { get; set; }
        public long? CollectedPenaltyAmount { get; set; }
        public long? CollectedProfitAmount { get; set; }
        public long? CollectedMainAmount { get; set; }
        public bool ISComputeCollectedAmount { get; set; }
        public DateTime? OldEndDate { get; set; }


    }
}
