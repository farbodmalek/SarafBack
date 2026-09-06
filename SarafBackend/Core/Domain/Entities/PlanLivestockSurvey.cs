using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Domains.Supervision.Entities
{
    [Table("PlanLivestockSurvey", Schema = "survey")]
    public  class PlanLivestockSurvey 
    {    
        [Key]
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public Survey Survey { get; set; }

        //---دفترچه دامداری
        public bool LivestockBooklet { get; set; }     
        
        //--پروانه دامداری
        public bool LivestockLicense { get; set; }   
        
        //---بیمه نامه دام
        public bool LivestockInsurance { get; set; }
        public System.DateTime? InsuranceDate { get; set; }


        //--- تعداد دام بیمه شده
        public int? NumberOfInsuredLivestock { get; set; }

        //--- نوع دام
        public byte LivestockTypeId { get; set; }  
        

        //--تعداد نرها
        public int NumberOfMaleLivestock { get; set; }

        //-- تعداد ماده ها
        public int? NumberOfFemaleLivestock { get; set; }    
    }
}
