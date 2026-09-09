using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CommonLibrary.Core.Domain.Entities.Users;

namespace CommonLibrary.Core.Domain.Entities
{
    [Table("Branches", Schema = "Core")]
    public class Branch : BaseEntity
    {
        [Required]
        public int SupervisorCode { get; set; }
        [Required]
        public string SupervisorName { get; set; }
        [Required]
        public int BranchCode { get; set; }
        [Required]
        [StringLength(50)]
        public string BranchName { get; set; }
        public string? RelatedSupervisors { get; set; }
        public ICollection<UserBranch> UserBranches { get; set; }
        [StringLength(50)]
        public string? Latitude { get; set; }
        [StringLength(50)]
        public string? Longitude { get; set; }
        public string? CityName { get; set; }
        public string? ProvinceName { get; set; }
        public bool IsMetropolis { get; set; }
    }
}
