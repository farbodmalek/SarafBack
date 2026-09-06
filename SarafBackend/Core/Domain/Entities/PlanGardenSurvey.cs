using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Domains.Supervision.Entities
{
    [Table("PlanGardenSurvey", Schema = "survey")]
    public class PlanGardenSurvey 
    {
        [Key]
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public Survey Survey { get; set; }

        //--نوع مالکیت
        public byte OwnerTypeId { get; set; }
 

        //---نوع محصول
        public byte ProductTypeId { get; set; }
     


        //--وسعت زمین زراعی
        public double LandArea { get; set; }


        //-- مساحت زمین زیر کشت
        public double CultivatedLandArea { get; set; }


        //---بیمه نامه کشاورزی دارد
        public bool HasAgriculturalInsurance { get; set; }



        //--- تاریخ پایان بیمه نامه کشاورزی
        public DateTime? EndOfAgriculturalInsurance { get; set; }
    }
}
