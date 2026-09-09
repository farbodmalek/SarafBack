using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.Users
{
    [Table("Roles", Schema = "Core")]
    public class Role
    {

        public Role()
        {
            UserRoles = new HashSet<UserRoles>();
        }

        [Key, Column("RoleID")]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }
        public bool IsLegal { get; set; }
        public bool IsActive { get; set; }
        public ICollection<UserRoles> UserRoles { get; set; }
        public ICollection<RoleActivity> RoleActivity { get; set; }
    }
}
