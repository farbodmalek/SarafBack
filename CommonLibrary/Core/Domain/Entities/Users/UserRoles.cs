using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.Users
{
    [Table("UserRoles", Schema = "Core")]
    public class UserRoles 
    {
        [Key, Column("UserRoleID")]
        public int ID { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public Role Role { get; set; }
        public User User { get; set; }
    }
}
