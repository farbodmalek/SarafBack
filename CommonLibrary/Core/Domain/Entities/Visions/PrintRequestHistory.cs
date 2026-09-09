using CommonLibrary.Core.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibrary.Core.Domain.Entities.Visions
{
    [Table("PrintRequestHistory", Schema = "Vision")]
    public class PrintRequestHistory : BaseVision
    {
        public int UserId { get; set; }
        public int SupervisorCode { get; set; }
        public DateTime RequestDate { get; set; }
        public int RequestMonth { get; set; }
        public int RequestYear { get; set; }
        public ICollection<LoanActor>? Actors { get; set; }
        public int? PrintRequestStatusId { get; set; }
    }
}
