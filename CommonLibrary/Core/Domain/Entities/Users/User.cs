using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.Users
{
    [Table("Users", Schema = "Core")]
    public class User 
    {
        [Key, Column("UserID")]
        public int ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? FullUserName { get; set; }
        public string? NationalCode { get; set; }
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string? Password { get; set; }
        public string? HashedPassword { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool isActive { get; set; }
        public virtual ICollection<UserRoles> UserRoles { get; set; }
        public ICollection<UserBranch> UserBranches { get; set; }
        public string? TelNumber { get; set; }
        public string? MobileNumber { get; set; }
        public DateTime? StartEmploymentDate { get; set; }
        public DateTime? EndEmploymentDate { get; set; }
        public bool IsLegalUser { get; set; }
        public int? SematId { get; set; }
    }
}
