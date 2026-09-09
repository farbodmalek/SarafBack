using CommonLibrary.Core.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibrary.Core.Domain.Entities.Loans
{
    [Table("InvalidLoan", Schema = "Core")]
    public class InvalidLoan : BaseVision
    {
        public int BranchCode { get; set; }
        public int LoanMinorTypeId { get; set; }
        public int CustomerNumber { get; set; }
        public int LoanSerial { get; set; }
        public string LoanNumber { get; set; }
        public DateTime RBDATE { get; set; }
        public int RollBackActionTypeId { get; set; }
        public string Description { get; set; }
        public string LoanStatusTypeId { get; set; }
        public int RN { get; set; }
        public bool IsDisabled { get; set; }
        public int? LoanId { get; set; }
    }
}
