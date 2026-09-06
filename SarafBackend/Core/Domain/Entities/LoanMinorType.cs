using CommonLibrary.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanMonitoringMicroService.Core.Domain.ViewModel.Survey
{
    [Table("LoanMinorTypes", Schema = "Core")]
    public class LoanMinorType 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int LoanMajorTypeId { get; set; } //fk
        public int? LegalLoanMinorTypeId { get; set; }
        public string Desc { get; set; }
        public ICollection<Loan> Loans { get; set; }
        public ICollection<SurveyReferenceBaseInfo> SurveyReferenceBaseInfos { get; set; }
    }
}
