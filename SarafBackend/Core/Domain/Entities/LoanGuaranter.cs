using CommonLibrary.Core.Domain.Entities.Common;
using CommonLibrary.Core.Domain.Entities.Customers;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Core.Domain.Entities
{
    [Table("LoanGuaranties", Schema = "Core")]
    public class LoanGuaranter : BaseLoan
    {
        public int Id { get; set; }
        public int? LoanId { get; set; } //fk
        public int GuarantorCustomerNumber { get; set; }
        public int? GuarantorCustomerId { get; set; } //fk
        /// <summary>
        /// نوع رابطه
        /// </summary>
        public int RelationTypeId { get; set; } // table
        [ForeignKey(nameof(GuarantorCustomerId))]                             //    [NotMapped]
        public Customer? Customer { get; set; }
        public int? Rn { get; set; }
        public DateTime? Cdate { get; set; }
    }
}
