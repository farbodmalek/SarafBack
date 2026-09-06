using CommonLibrary.Infrastructure.Utils;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Core.Domain.Entities
{
    [Table("Cartable", Schema = "survey")]
    public class CartableSurvey
    {
        public int Id { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public byte? CartableStatusTypeId { get; set; }
        public int? UserId { get; set; }
        public int? LoanId { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? CartableConfirmationId { get; set; }
        public int? LoanSurveyEconomicTypeId { get; set; }
        public int? PlanNoId { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ReferenceUserId { get; set; }
        public string? Description { get; set; }
    }
}
