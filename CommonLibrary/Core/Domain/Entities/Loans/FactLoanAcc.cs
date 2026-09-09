using CommonLibrary.Core.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibrary.Core.Domain.Entities.Loans
{
    [Table("FactLoanAcc", Schema = "Core")]

    public class FactLoanAcc : BaseFact
    {
        public bool IsShowing { get; set; }

    }
}
