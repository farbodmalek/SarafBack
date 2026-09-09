using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibrary.Core.Domain.Entities.Survey
{
    [Table("LoanSurveyEconomicType", Schema = "survey")]
    public class LoanSurveyEconomicType
    {
        [Key]
        public byte Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public int LoanEconomicTypeId { get; set; }
    }
}
