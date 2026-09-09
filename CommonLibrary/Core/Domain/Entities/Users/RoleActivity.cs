using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibrary.Core.Domain.Entities.Users
{
    [Table("RoleActivities", Schema = "Core")]
    public class RoleActivity : BaseEntity
    {
        public int ActivityID { get; set; }
        public Activity Activity { get; set; }
        public int RoleID { get; set; }
        public Role Role { get; set; }
    }
}
