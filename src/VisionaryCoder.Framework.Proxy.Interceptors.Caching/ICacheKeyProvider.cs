using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Caching;

/// <summary>
/// Interface for generating cache keys based on proxy context.
/// </summary>
public interface ICacheKeyProvider
{
    /// <summary>
    /// Generates a cache key for the given context and response type.
    /// </summary>
    /// <typeparam name="T">The response type.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <returns>A unique cache key.</returns>
    string GenerateKey<T>(ProxyContext context);
}