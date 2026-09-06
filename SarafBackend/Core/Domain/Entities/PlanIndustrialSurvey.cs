using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Domains.Supervision.Entities
{
    [Table("PlanIndustrialSurvey", Schema = "survey")]
    public  class PlanIndustrialSurvey 
    {    
        [Key]
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public Survey Survey { get; set; }
        public bool HasWorkPermission { get; set; }
        public byte OwnerTypeId { get; set; }
        public byte? PresenceTypeId { get; set; }    

    }
}
