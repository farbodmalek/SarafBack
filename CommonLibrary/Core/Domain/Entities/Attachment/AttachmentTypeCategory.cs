using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.Attachment
{
    [Table("AttachmentTypeCategory", Schema = "Core")]
    public class AttachmentTypeCategory
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string AttachmentTypeCategoryName { get; set; }

        public int AttachmentTypeCategoryCode { set; get; }

        public int AttachmentTypeCategoryID { get; set; }

        [StringLength(250)]
        public string AttachmentTypeCategoryDesc { get; set; }

        public ICollection<AttachmentType> AttachmentTypes { get; set; }
    }
}
