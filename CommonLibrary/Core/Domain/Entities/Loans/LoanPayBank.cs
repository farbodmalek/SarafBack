using CommonLibrary.Core.Domain.Entities.Loans;
using CommonLibrary.Core.Domain.Entities.Survey;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CommonLibrary.Core.Domain.Entities.Customers;

namespace CommonLibrary.Core.Domain.Entities
{
    [Table("LoanPayBanks", Schema = "Core")]
    public class LoanPayBank
    {
        public int Id { get; set; }
        public int BranchCode { get; set; }
        public int LoanMinorTypeId { get; set; }
        public int CustomerNumber { get; set; }
        public string LoanNumber { get; set; }
        public int LoanId { get; set; }
        public int LoanSerial { get; set; }
        public long? LoanKey { get; set; }
        public long PayAmount { get; set; }
        public DateTime PayDate { get; set; }
       
    }
}
