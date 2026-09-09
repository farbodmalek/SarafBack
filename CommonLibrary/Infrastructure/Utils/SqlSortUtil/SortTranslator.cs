using CommonLibrary.Core.Domain.Dto.Common;

namespace CommonLibrary.Infrastructure.Utils.SqlSortUtil
{
    public sealed class SortTranslator
    {
        private SortTranslator()
        {

        }
        private static SortTranslator _instance = null;
        private static Dictionary<string, string> _sortDictionary;
        public static SortTranslator Instance(Dictionary<string, string> sortDictionary)
        {
            _sortDictionary = sortDictionary;
            if (_instance == null)
                _instance = new SortTranslator();
            return _instance;
        }
        public string makeSortQuery(SortDTO? sort)
        {
            var query = sort == null ?
                makeDefaultSort() :
                makeSortListString(sort);
            return " ORDER BY " + query;
        }

        public string makeSortQueryPWA(SortDTO? sort)
        {
            var query = sort == null ?
                makeDefaultSort() :
                makeSortListString(sort);
            return " ORDER BY ID desc ";
        }

        private string makeDefaultSort()
        {
            return _sortDictionary.First().Value;
        }
        private string makeSortListString(SortDTO sort)
        {
            var query = makeDefaultSort();
            sort.Field = sort.Field.Contains("Fa") ? sort.Field.Remove(sort.Field.Length - 2) : sort.Field;
            if (_sortDictionary.Any(d => d.Key.ToLower() == sort.Field.ToLower()))
            {
                query = $"{_sortDictionary.Single(d => d.Key.ToLower() == sort.Field.ToLower()).Value} {sort.Dir}";
            }
            return query;
        }
    }
}
