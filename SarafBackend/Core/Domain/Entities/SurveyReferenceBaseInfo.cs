using CommonLibrary.Core.Domain.Entities.Common;
using LoanMonitoringMicroService.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Core.Domain.ViewModel.Survey
{
    [Table("SurveyReferenceBaseInfo", Schema = "survey")]
    public class SurveyReferenceBaseInfo : BaseVision
    {
        public int LoanMinorTypeId { get; set; }
        public bool IsActive { get; set; }
        public ICollection<SurveyReferenceReagent> SurveyReferenceReagents { get; set; }
        public ICollection<SurveyReferenceLoanAmount> SurveyReferenceLoanAmounts { get; set; }
        public ICollection<SurveyReferenceContractDate> SurveyReferenceContractDates { get; set; }
        
        public ICollection<AllowedFirstTimeSupervisionRole> AllowedFirstTimeSupervisionRoles { get; set; }
    }

}
