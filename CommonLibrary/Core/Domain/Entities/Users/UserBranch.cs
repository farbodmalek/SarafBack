using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace CommonLibrary.Core.Domain.Entities.Users
{
    [Table("UserBranches", Schema = "Core")]
    public class UserBranch
    {
        [Key, Column("UserBranchID")]
        public int ID { get; set; }

        [ForeignKey("Branch")]
        public int BranchId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual Branch Branch { get; set; }
        public virtual User User { get; set; }
    }
}
