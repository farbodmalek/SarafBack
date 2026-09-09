using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibrary.Core.Domain.Entities.Common
{
    public class BaseFactVision : BaseFact
    {
        public bool IsReference { get; set; }
        public DateTime? ReferenceTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
