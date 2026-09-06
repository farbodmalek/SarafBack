using CommonLibrary.Core.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Core.Domain.ViewModel.Survey
{
    [Table("NotSurveyReferenceReagent", Schema = "survey")]
    public class SurveyReferenceReagent : BaseVision
    {
        public int SurveyReferenceBaseInfoId { get; set; }
        public int LoanMinorTypeId { get; set; }
        public int CustomerId { get; set; }   
        public bool IsActive { get; set; }
    }

}
