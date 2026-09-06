using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

namespace LoanMonitoringMicroService.Core.Domain.Entities
{
   
    [Table("OutsideSurvey", Schema = "survey")]
    public class OutsideSurvey
    {
        
        public int Id { get; set; }
        public int PlanActivationTypeId { get; set; }
        public int LoanId { get; set; }
        public DateTime? SurveyDate { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;  
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
        public string? PlanNoName { get; set; }
        public int numberOfInsurdPerson { get; set; }
        public int numberOfJobsCreated { get; set; }
        public int numberOfJobsObligated { get; set; }
        public DateTime? DeadLineDate { get; set; }
        public string? ReasonSubmit { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public int? CartableId { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public bool? IsDeleted { get; set; } 
        //public ICollection<Attachment> Attachments { get; set; }
        //public PlanActivationType PlanActivationType { get; set; }
    }
}

