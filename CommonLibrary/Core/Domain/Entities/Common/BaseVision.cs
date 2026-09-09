using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.Common
{
    public class BaseVision
    {
        [Key]
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
