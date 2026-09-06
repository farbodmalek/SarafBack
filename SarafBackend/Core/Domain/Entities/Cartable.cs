using PaymentMonitoring.Domains.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Core.Domain.Entities
{
    //[Table("Cartable", Schema = "survey")]

    //public class Cartable 
    //{

    //    public int Id { get; set; }

    //    public int LoanId { get; set; }
    //    /// <summary>
    //    /// کارشناس ناظر
    //    /// </summary>
    //    public int UserId { get; set; }
    //    public int? ReferenceUserId { get; set; }
         
    //    /// <summary>
    //    /// تاریخ ارجاع به نظارت--زمان ارجاع به کارشناس
    //    /// </summary>
    //    public DateTime ReferenceDate { get; set; }
         
    //    /// <summary>
    //    /// مهلت اقدام
    //    /// </summary>
    //    public DateTime? ExpireDate { get; set; }
    //    public DateTime? EndDate { get; set; }
    //    /// <summary>
    //    /// مرحله نظارت
    //    /// </summary> 
    //    public byte CartableStatusTypeId { get; set; }
    //    public User User { get; set; }
    //    //public ICollection<LoanPlan> LoanPlans { get; set; }
    //    //public ICollection<Survey> Surveys { get; set; } = new List<Survey>();
    //    //public ICollection<ReferenceHistory> ReferenceHistories { get; set; }
    //    //public ICollection<SurveyReferenceHistory> SurveyReferenceHistories { get; set; }
    //    public int? CartableConfirmationId { get; set; }
    //    public byte? LoanSurveyEconomicTypeId { get; set; }
    //    public int? PlanNoId { get; set; }
    //    [MaxLength(500)]
    //    public string Description { get; set; }
    //}
}
