using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Core.Domain.Dto
{
    public class MapResultList<Input, Output> where Input : class where Output : class
    {
        public static ResultList<Output> Mapping(ResultList<Input> list, IMapper mapper)
        {
            var result = new ResultList<Output>
            {
                TotalRows = list.TotalRows,
                Results = list.Results.Select(p => mapper.Map<Output>(p)).ToList()
            };
            return result;
        }
    }
}
