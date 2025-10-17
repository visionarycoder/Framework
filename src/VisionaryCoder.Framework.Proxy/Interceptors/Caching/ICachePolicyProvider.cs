using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Caching;

/// <summary>
/// Interface for determining cache policies based on proxy context.
/// </summary>
public interface ICachePolicyProvider
{
    /// <summary>
    /// Gets the cache policy for the given context.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>The cache policy to apply.</returns>
    CachePolicy GetPolicy(ProxyContext context);
}