using CommonLibrary.Core.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibrary.Core.Domain.Entities.Visions
{
    [Table("ActorActionFlow", Schema = "Vision")]
    public class ActorActionFlow : BaseVision
    {
        public int ActorId { get; set; }
        [ForeignKey(nameof(ActorId))]
        public LoanActor Actor { get; set; }
        public DateTime ConfirmationDate { get; set; }
        public int ConfirmationUserId { get; set; }
        public int ActorStatusTypeId { get; set; }
        public string Description { get; set; }
    }
}
