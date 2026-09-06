using CommonLibrary.Core.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Core.Domain.Entities
{
    [Table("SurveyReferenceLoanAmount", Schema = "survey")]
    public class SurveyReferenceLoanAmount : BaseVision
    {
        public int SurveyReferenceBaseInfoId { get; set; }
        public int LoanMinorTypeId { get; set; }
        public int? AmountMin { get; set; }
        public int? AmountMax { get; set; }
        public bool IsActive { get; set; }
    }
}
