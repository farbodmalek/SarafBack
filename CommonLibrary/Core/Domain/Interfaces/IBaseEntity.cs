using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Core.Domain.Interfaces
{
    public interface IBaseEntity
    {
        /// <summary>
        /// شناسه
        /// </summary>
        int ID { get; set; }
    }
}
