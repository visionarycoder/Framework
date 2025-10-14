// VisionaryCoder.Framework.Proxy.Caching

using Microsoft.Extensions.Caching.Memory;

using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Caching;

public sealed class MemoryProxyCache(IMemoryCache cache) : IProxyCache
{
    public Task<T?> GetAsync<T>(string key)
    {
        if (cache.TryGetValue(key, out var obj) && obj is T typed)
        {
            return Task.FromResult<T?>(typed);
        }
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        if (expiration.HasValue)
            cache.Set(key, value!, expiration.Value);
        else
            cache.Set(key, value!);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }
}
