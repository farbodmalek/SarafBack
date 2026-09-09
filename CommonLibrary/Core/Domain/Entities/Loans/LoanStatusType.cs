using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Core.Domain.Entities.Loans
{
    public class LoanStatusType
    {
        public string Id { get; set; }
        public string Desc { get; set; }
        public int DisplayOrder { get; set; }
        public ICollection<Loan> Loans { get; set; }
    }
}
