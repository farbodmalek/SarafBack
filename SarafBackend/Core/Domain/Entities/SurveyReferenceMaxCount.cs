using CommonLibrary.Core.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Core.Domain.Entities
{
    [Table("SurveyMaxCount", Schema = "survey")]
    public class SurveyMaxCount : BaseVision
    {
        public decimal LoanAmountMin { get; set; }
        public decimal LoanAmountMax { get; set; }
        public int MaxReferenceCountByYear { get; set; }
        public bool IsActive { get; set; }
    }
}
