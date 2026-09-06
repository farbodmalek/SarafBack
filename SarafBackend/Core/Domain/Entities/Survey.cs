using CommonLibrary.Core.Domain.Entities.Common;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Survey;
using LoanMonitoringMicroService.Domains.Core.Supervision.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LoanMonitoringMicroService.Domains.Supervision.Entities
{
    [Table("Survey", Schema = "survey")]
    public class Survey : BaseVision
    {
        public Survey()
        {
            this.PlanLivestockSurvey = new HashSet<PlanLivestockSurvey>();
            this.PlanGardenSurvey = new HashSet<PlanGardenSurvey>();
            this.PlanIndustrialSurvey = new HashSet<PlanIndustrialSurvey>();
            this.PlanServiceSurvey = new HashSet<PlanServiceSurvey>();
        }


        //---شمار ه کارتابل
        public int CartableId { get; set; }
        public DateTime? EndOfActivationDate { get; set; }
        public bool ConstructionApproval { get; set; }
        public byte? ConstructionPercentageProgress { get; set; }
        public string? ConstructionDescription { get; set; }
        /// <summary>
        /// 0 - خریداری نشده    
        /// 1 - خریداری شده
        /// 2 - تجهیزات ندارد
        /// </summary>
        public int IsEquipmentBought { get; set; }
        public bool? IsFactorMatch { get; set; }
        public byte? EquipmentTypeId { get; set; }
        [StringLength(500)]
        public string? EquipmentDescription { get; set; }
        [StringLength(500)]
        public string? SurveyReport { get; set; }
        [StringLength(500)]
        public string? CustomerOffer { get; set; }
        public int? NumberOfJobsCreated { get; set; }
        public int? NumberOfInsurdPerson { get; set; }
        public bool IsLocationConfirmed { get; set; }
        public int? LocationConfirmationUserId { get; set; }
        public DateTime? LocationConfirmationDate { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public int SurveyRateBaseInfoId { get; set; }
        public int PlanActivationTypeId { get; set; }
        public int SurveyStatusTypeId { get; set; }
        //public Cartable Cartable { get; set; }
        ////---زمان ثبت نظارت
        public DateTime SurveyDate { get; set; }

        public bool? IsOffline { get; set; }
        public DateTime? OfflineDate { get; set; }

        //public ICollection<Attachment> Attachments { get; set; }

        public ICollection<PlanLivestockSurvey> PlanLivestockSurvey { get; set; } 
        //--باغی / زراعی
        public ICollection<PlanGardenSurvey> PlanGardenSurvey { get; set; } 
        //--صنعتی
        public ICollection<PlanIndustrialSurvey> PlanIndustrialSurvey { get; set; } 
        //--خدمات
        public ICollection<PlanServiceSurvey> PlanServiceSurvey { get; set; } 

    }
}
