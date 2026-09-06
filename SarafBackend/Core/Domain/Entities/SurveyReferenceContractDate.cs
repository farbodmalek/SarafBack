using CommonLibrary.Core.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Core.Domain.Entities
{
    [Table("SurveyReferenceContractDate", Schema = "survey")]
    public class SurveyReferenceContractDate : BaseVision
    {
        public int SurveyReferenceBaseInfoId { get; set; }
        public int LoanMinorTypeId { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
