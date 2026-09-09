using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static Dapper.SqlMapper;
using System.Net.Mail;

namespace CommonLibrary.Core.Domain.Entities.Attachment
{
    [Table("AttachmentTypes", Schema = "Core")]
    public class AttachmentType
    {
        public int ID { get; set; }
        public string AttachmentTypeName { get; set; }
        public int AttachmentTypeCode { set; get; }
        public int AttachmentTypeCategoryID { get; set; }
        public AttachmentTypeCategory AttachmentTypeCategory { get; set; }
        public string AttachmentTypeDesc { get; set; }
        public ICollection<Attachments> Attachments { get; set; }
    }
}
