using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibrary.Core.Domain.Entities.Users
{
    [Table("Activities", Schema = "Core")]
    public class Activity : BaseEntity
    {
        public int ParentId { get; set; }
        public long Order { get; set; }
        public long Code { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string IconClass { get; set; }
        public bool IsActive { get; set; }
        public bool IsMenu { get; set; }
        public int? SystemType { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
