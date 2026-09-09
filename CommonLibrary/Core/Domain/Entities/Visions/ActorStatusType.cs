using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibrary.Core.Domain.Entities.Visions
{
    [Table("ActorStatusType", Schema = "Vision")]
    public class ActorStatusType
    {
        public int Id { get; set; }
        public string Desc { get; set; }
        public int DisplayOrder { get; set; }
    }
}
