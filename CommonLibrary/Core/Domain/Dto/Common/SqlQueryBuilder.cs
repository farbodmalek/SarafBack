
namespace CommonLibrary.Core.Domain.Dto.Common
{
    public class SqlQueryBuilder
    {
        public string SelectSection { get; set; }
        public string CountSection { get; set; }
        public string TableSection { get; set; }
        public string WhereSection { get; set; }
        public string PagingSection { get; set; }
        public string TotalQuery { get { return SelectSection + TableSection + WhereSection + PagingSection; } }
        public string TotalCountQuery { get { return CountSection + TableSection + WhereSection; } }
    }
}
