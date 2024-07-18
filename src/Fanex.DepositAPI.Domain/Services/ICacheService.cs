namespace Fanex.DepositAPI.Domain.Services;

public interface ICacheService
{
    void RemoveFromCache(string key);

    T GetValueFromCache<T>(string key, T defaultValue = default);

    void SetValueToCache<T>(string key, T value, TimeSpan? cacheTime);

    void SetValueToCacheWithoutExpired<T>(string key, T value);

    Task<T> GetOrCreateFromCache<T>(string key, Func<Task<T>> action, TimeSpan? cacheTime) where T : class;

    Task<T> GetOrCreateFromCacheWithoutExpired<T>(string key, Func<Task<T>> action) where T : class;
}