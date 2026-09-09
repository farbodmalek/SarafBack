
namespace CommonLibrary.Infrastructure.Utils.Extensions
{
    public static class ListExtention
    {
        public static string JoinToString(this List<int> list)
        {
            return string.Join(",", list);
        }
        public static string JoinStringListToString(this List<string> list)
        {
            if (list == null)
                return string.Empty;
            return string.Join(",", list);
        }
        public static string JoinStringListToStringWithComma(this List<string> list)
        {
            return string.Join(",'", list);
        }
    }
}
