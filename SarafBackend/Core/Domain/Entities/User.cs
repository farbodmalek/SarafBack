using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentMonitoring.Domains.Core.Entities
{
    [Table("Users", Schema = "Core")]
    public class User 
    {
        [Key, Column("UserID")]
        public int ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullUserName { get; set; }
        public string NationalCode { get; set; }
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; }
        public DateTime ExpireDate { get; set; }
        public Boolean isActive { get; set; }
    //    public virtual ICollection<UserRoles> UserRoles { get; set; }
    //    public ICollection<UserBranches> UserBranches { get; set; }
    //    [ForeignKey("UserId")]
    //    public ICollection<Actor> Actors { get; set; }
    //    [ForeignKey("ReferenceUserId")]
    //    public ICollection<Actor> FkReferenceUser { get; set; }
    //    [ForeignKey("RollbackUserId")]
    //    public ICollection<Actor> FkRollbackUser { get; set; }
    //    [ForeignKey("CreatedBy")]
    //    public ICollection<LoanActionLog> LoanActionLogs { get; set; }
    ////    public ICollection<Survey.Entities.Survey> Surveys { get; set; }
    //    public ICollection<Cartable> Cartables { get; set; }
    //    public string TelNumber { get; set; }
    //    public string MobileNumber { get; set; }
    //    public ICollection<ChangePasswordHistory> ChangePasswordHistorys { get; set; }
    //    public ICollection<CartableConfirmation> CartableConfirmations { get; set; }
    //    [ForeignKey("RequestUserId")]
    //    public ICollection<LegalReferenceInfo> LegalReferenceInfos { get; set; }   
    //    [ForeignKey("ConfirmationUserId")]
    //    public ICollection<LegalReferenceDetail> LegalReferenceDetails { get; set; }   
        public DateTime? StartEmploymentDate { get; set; }
        public DateTime? EndEmploymentDate { get; set; }
        public bool IsLegalUser { get; set; }
    }
}
