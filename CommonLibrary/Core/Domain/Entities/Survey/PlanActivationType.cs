using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.Survey
{
    [Table("PlanActivationType", Schema = "survey")]
    public class PlanActivationType 
    {
        [Key]
        public int ID { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
    }
}
