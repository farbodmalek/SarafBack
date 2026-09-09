using CommonLibrary.Core.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary.Core.Domain.Entities.PlanNo;

namespace CommonLibrary.Core.Domain.Entities.Survey
{
    [Table("Cartable", Schema = "survey")]
    public class Cartable 
    {
        [Key]
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public int LoanId { get; set; }
        /// <summary>
        /// کارشناس ناظر
        /// </summary>
        public int UserId { get; set; }
        public int? SurveyType { get; set; }
        public int? ReferenceUserId { get; set; }

        /// <summary>
        /// تاریخ ارجاع به نظارت--زمان ارجاع به کارشناس
        /// </summary>
        public DateTime ReferenceDate { get; set; }

        /// <summary>
        /// مهلت اقدام
        /// </summary>
        public DateTime? ExpireDate { get; set; }
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// مرحله نظارت
        /// </summary> 
        public byte CartableStatusTypeId { get; set; }
        //public User? User { get; set; }
        //public ICollection<LoanPlan> LoanPlans { get; set; }
        //public ICollection<Survey> Surveys { get; set; } = new List<Survey>();
        //public ICollection<ReferenceHistory> ReferenceHistories { get; set; }
        //public ICollection<SurveyReferenceHistory> SurveyReferenceHistories { get; set; }
        public int? CartableConfirmationId { get; set; }
        public byte? LoanSurveyEconomicTypeId { get; set; }
        public int? PlanNoId { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public bool? IsNotScheduledReferred { get; set; }
    }
}
