using CommonLibrary.Core.Domain.Entities.Survey;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.PlanNo
{
    [Table("LoanPlan", Schema = "survey")]
    public class LoanPlan
    {
        [Key]
        public int ID { get; set; }
        public int CartableId { get; set; }
        public int LoanId { get; set; }
        public byte? LoanSurveyEconomidTypeId { get; set; }
        public bool MaritalStatusId { get; set; }
        public byte ResidentTypeId { get; set; }
        public bool? GenderType { get; set; }
        public bool IsFamilySupervisor { get; set; }
        [StringLength(11, MinimumLength = 11)]
        public string MobileNo { get; set; }
        [StringLength(15)]
        public string Phone { get; set; }
        public byte PlanTypeId { get; set; }
        [StringLength(50)]
        public string Latitude { get; set; }
        [StringLength(50)]
        public string Longitude { get; set; }
        [StringLength(500)]
        public string? CityName { get; set; }
        [StringLength(50)]
        public string? VillageName { get; set; }
        public int PlanNoId { get; set; }
        
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public int EducationTypeId { get; set; }
        public int LoanSystemTypeId { get; set; }
        public long? WorkshopCode { get; set; }
        public int? InsuredPersonsCount { get; set; }
        public int? InsuranceTypeId { get; set; }
        [MaxLength(300)]
        public string? OtherPlanNo { get; set; }
        public string Address { get; set; }
        public Cartable Cartable { get; set; }
        public Loan Loan { get; set; }
        //public ICollection<SurveyWageLoanPlan> SurveyWageLoanPlans { get; set; }
        //[ForeignKey("LoanSurveyEconomidTypeId")]
        //public LoanSurveyEconomicType LoanSurveyEconomidType { get; set; }
    }
}
