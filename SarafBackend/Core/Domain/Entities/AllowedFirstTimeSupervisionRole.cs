using CommonLibrary.Core.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Core.Domain.Entities
{
    [Table("NotAllowedSupervisionRoles", Schema = "survey")]
    public class AllowedFirstTimeSupervisionRole : BaseVision
    {
        public int RoleId { get; set; }
        public int SurveyReferenceBaseInfoId { get; set; }
        public int LoanMinorTypeId { get; set; }
        public bool IsActive { get; set; }
    }
}
