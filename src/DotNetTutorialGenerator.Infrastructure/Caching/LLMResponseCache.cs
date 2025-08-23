using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DotNetTutorialGenerator.Infrastructure.Caching
{
    public class LLMResponseCache : ILLMResponseCache
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<LLMResponseCache> _logger;

        public LLMResponseCache(IMemoryCache cache, ILogger<LLMResponseCache> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration)
        {
            if (_cache.TryGetValue(key, out T cachedValue))
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return cachedValue;
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            var value = await factory();
            _cache.Set(key, value, expiration);
            return value;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out T cachedValue))
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return Task.FromResult<T?>(cachedValue);
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            return Task.FromResult<T?>(default(T));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            _cache.Set(key, value, expiration);
            _logger.LogDebug("Set cache entry for key: {Key}", key);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            _logger.LogDebug("Removed cache entry for key: {Key}", key);
            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            // Note: IMemoryCache doesn't have a clear method, so we'll just log
            _logger.LogInformation("Cache clear requested (IMemoryCache doesn't support full clear)");
            return Task.CompletedTask;
        }
    }
}
