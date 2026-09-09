using CommonLibrary.Core.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibrary.Core.Domain.Entities.Loans
{
    [Table("LoanActionLog", Schema = "Core")]
    public class LoanActionLog
    {
        public int Id { get; set; }
        public int loanId { get; set; }
        public string ActionType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? LoanActionLogTypeId { get; set; }
        //public User User { get; set; }
    }
}
