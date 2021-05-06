using System;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

namespace ToDoList.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache, string recordId, T data,
                                                   TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60),
                SlidingExpiration = slidingExpireTime
            };

            string jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordId, jsonData, options);
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            string jsonData = await cache.GetStringAsync(recordId);

            return jsonData is not null
                ? JsonSerializer.Deserialize<T>(jsonData)
                : default;
        }
    }
}
