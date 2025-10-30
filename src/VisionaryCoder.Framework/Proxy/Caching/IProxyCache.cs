namespace VisionaryCoder.Framework.Proxy.Caching;

/// <summary>
/// Defines a contract for proxy caching operations.
/// </summary>
public interface IProxyCache
{
    /// <summary>
    /// Gets a cached response for the given key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>The cached response, or null if not found.</returns>
    Task<ProxyResponse<T>?> GetAsync<T>(string key);
    /// <summary>
    /// Sets a proxyResponse in the cache with the given key and expiration.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="proxyResponse">The proxyResponse to cache.</param>
    /// <param name="expiration">The cache expiration time.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync<T>(string key, ProxyResponse<T> proxyResponse, TimeSpan expiration);
}