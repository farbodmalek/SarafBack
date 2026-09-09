using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Core.Domain.Dto.Common
{
    public class QueryParam
    {
        protected delegate string AddQueryParam(object value);
        protected Dictionary<string, AddQueryParam>? AddQueryParamMap { get; set; }
    }
}
