// VisionaryCoder.Framework.Proxy.Caching

using Microsoft.Extensions.Caching.Memory;

namespace VisionaryCoder.Framework.Proxy.Caching;

public sealed class MemoryProxyCache(IMemoryCache cache) : IProxyCache
{
    /// <summary>
    /// Gets a cached response for the given key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>The cached response, or null if not found.</returns>
    public Task<ProxyResponse<T>?> GetAsync<T>(string key)
    {
        if (cache.TryGetValue(key, out object? obj) && obj is ProxyResponse<T> typed)
        {
            return Task.FromResult<ProxyResponse<T>?>(typed);
        }
        return Task.FromResult<ProxyResponse<T>?>(null);
    }

    /// <summary>
    /// Sets a proxyResponse in the cache with the given key and expiration.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="proxyResponse">The proxyResponse to cache.</param>
    /// <param name="expiration">The cache expiration time.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SetAsync<T>(string key, ProxyResponse<T> proxyResponse, TimeSpan expiration)
    {
        cache.Set(key, proxyResponse, expiration);
        return Task.CompletedTask;
    }
}
