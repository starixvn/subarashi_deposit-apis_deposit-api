using Fanex.DepositAPI.Domain.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Fanex.DepositAPI.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    private readonly int DefaultExpiredMinutes = 15;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrCreateFromCache<T>(string key, Func<Task<T>> action, TimeSpan? cacheTime) where T : class
    {
        if (!_cache.TryGetValue(key, out T data))
        {
            data = await action();
            if (data is not null)
            {
                SetValueToCache(key, data, cacheTime);
            }
        }
        return data;
    }

    public async Task<T> GetOrCreateFromCacheWithoutExpired<T>(string key, Func<Task<T>> action) where T : class
    {
        if (!_cache.TryGetValue(key, out T data))
        {
            data = await action();
            if (data is not null)
            {
                SetValueToCacheWithoutExpired(key, data);
            }
        }
        return data;
    }

    public T GetValueFromCache<T>(string key, T defaultValue = default)
    {
        if (!_cache.TryGetValue(key, out T data))
        {
            return data;
        }

        return defaultValue;
    }

    public void RemoveFromCache(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key), "Key should not be empty!");
        }

        _cache.Remove(key);
    }

    public void SetValueToCache<T>(string key, T value, TimeSpan? cacheTime)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key), "Key should not be empty!");
        }

        if (!Equals(value, default(T)))
        {
            _cache.Set(key, value, cacheTime ?? TimeSpan.FromMinutes(DefaultExpiredMinutes));
        }
    }

    public void SetValueToCacheWithoutExpired<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key), "Key should not be empty!");
        }

        if (!Equals(value, default(T)))
        {
            var options = new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.NeverRemove,
            };

            _cache.Set(key, value, options);
        }
    }
}