namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Defines a contract for generating cache keys.
/// </summary>
public interface ICacheKeyProvider
{
    /// <summary>
    /// Generates a cache key for the given context.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>The generated cache key.</returns>
    string GenerateKey(ProxyContext context);
}
