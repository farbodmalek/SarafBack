
namespace CommonLibrary.Infrastructure.Utils.Extensions
{
    public static class ObjectExtension
    {
        public static List<string> GetInitializedFieldObject(this object obj)
        {
            var filterItemList = new List<string>();
            var fields = obj.GetType().GetProperties().Where(x => x != null).ToList();

            foreach (var field in fields)
            {
                if (field.GetValue(obj) != null)
                    filterItemList.Add(field.Name);
            }
            return filterItemList;
        }
        public static Dictionary<string, object> GetFieldProperties(this object obj)
        {
            var filterItemList = new Dictionary<string, object>();
            var fields = obj.GetType().GetProperties().Where(x => x != null).ToList();

            foreach (var field in fields)
            {
                if (field.GetValue(obj) != null)
                {
                    filterItemList.Add(field.Name, field.GetValue(obj));
                    //if ((List<int>)(field.GetValue(obj)) != null && ((List<object>)(field.GetValue(obj))).Count > 0)
                    //    filterItemList.Add(field.Name, field.GetValue(obj));
                    //else
                    //    filterItemList.Add(field.Name, field.GetValue(obj));
                }
            }
            return filterItemList;
        }
    }
}
