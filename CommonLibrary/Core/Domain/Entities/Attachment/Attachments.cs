using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.Attachment
{
    [Table("Attachments", Schema = "Core")]
    public class Attachments
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string? FileName { get; set; }
        [StringLength(100)]
        public string? Desc { get; set; }
        [StringLength(500)]
        public string? Path { get; set; }
        [StringLength(100)]
        public string FileType { get; set; }
        public int? UserID { get; set; }
        public int? LoanID { get; set; }
        public int? OutsideSurveyId { get; set; }
        public int? EnactmentId { get; set; }
        public int? LoanActionID { get; set; }
        public int? SurveyId { get; set; }
        public int? LegalReferenceInfoId { get; set; }
        public int? CustomerHeadID { get; set; }
        public int AttachmentTypeID { get; set; }
        //public AttachmentType AttachmentType { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public string PictureName { get; set; }
        // public File[] Files { get; set; }
        public Guid? Guid { get; set; }
        [StringLength(500)]
        public string? OldPath { get; set; }
    }
}
