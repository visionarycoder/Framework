namespace VisionaryCoder.Framework.Proxy.Caching;

/// <summary>
/// Defines a contract for cache policy providers.
/// </summary>
public interface ICachePolicyProvider
{
    /// <summary>
    /// Gets the cache expiration for the given context.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>The cache expiration time, or null if no caching should be applied.</returns>
    TimeSpan? GetExpiration(ProxyContext context);
    /// Determines whether the operation should be cached.
    /// <returns>True if the operation should be cached; otherwise, false.</returns>
    bool ShouldCache(ProxyContext context);
}
