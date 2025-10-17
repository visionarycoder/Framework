using Microsoft.Extensions.Caching.Memory;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Caching;

/// <summary>
/// Represents a cache policy for proxy operations.
/// </summary>
public class CachePolicy
{
    /// <summary>
    /// Gets or sets a value indicating whether caching is enabled.
    /// </summary>
    public bool IsCachingEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the cache duration.
    /// </summary>
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the cache priority.
    /// </summary>
    public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;

    /// <summary>
    /// Gets or sets a function to determine if a response should be cached.
    /// </summary>
    public Func<object, bool> ShouldCache { get; set; } = _ => true;

    /// <summary>
    /// Gets or sets a function to determine if a cached response should be refreshed.
    /// </summary>
    public Func<object, bool> ShouldRefresh { get; set; } = _ => false;
}