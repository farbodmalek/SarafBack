using CommonLibrary.Core.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace CommonLibrary.Infrastructure.Utils.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public List<string> GetListData<T>()
        {
            try
            {
                var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
                var collection = field.GetValue(_memoryCache) as ICollection<T>;
                var items = new List<string>();
                if (collection != null)
                    foreach (var item in collection)
                    {
                        var methodInfo = item.GetType().GetProperty("Key");
                        var val = methodInfo.GetValue(item);
                        items.Add(val.ToString());
                    }
                return items;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public T GetData<T>(string key)
        {
            try
            {
                T item = (T)_memoryCache.Get(key);
                return item;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            bool res = true;
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    _memoryCache.Set(key, value, expirationTime);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return res;
        }
        public void RemoveData(string key)
        {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    _memoryCache.Remove(key);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
