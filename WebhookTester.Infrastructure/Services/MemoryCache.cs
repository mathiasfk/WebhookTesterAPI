using Microsoft.Extensions.Caching.Memory;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.Infrastructure.Services
{
    public class MemoryCache<T>(IMemoryCache memoryCache) : ICache<T>
    {
        public Task SetAsync(string key, T value, TimeSpan? expiration = null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30),
                SlidingExpiration = expiration ?? TimeSpan.FromMinutes(5)
            };

            memoryCache.Set(key, value, cacheEntryOptions);
            return Task.CompletedTask;
        }

        public Task<T?> GetAsync(string key)
        {
            memoryCache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        public Task RemoveAsync(string key)
        {
            memoryCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
