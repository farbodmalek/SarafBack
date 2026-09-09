using CommonLibrary.Core.Domain.Entities.Survey;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Dapper.SqlMapper;

namespace CommonLibrary.Domains.Core.Supervision.Entities
{
    [Table("LoanPlanLocationState", Schema = "survey")]
    public class LoanPlanLocationState 
    {
        [Key]
        public int ID { get; set; }
        public int LoanPlanId { get; set; }
        [StringLength(500)]
        public string Address { get; set; }
        [StringLength(50)]
        public string Latitude { get; set; }
        [StringLength(50)]
        public string Longitude { get; set; }
        public int ChangeLoanPlanStateId { get; set; }
        public ChangeLoanPlanState ChangeLoanPlanState { get; set; }
        public int? CartableId { get; set; }
    }
}