using Microsoft.Extensions.Caching.Memory;

namespace VisionaryCoder.Framework.Proxy.Caching;
/// <summary>
/// Represents a cache policy for proxy operations.
/// </summary>
public class CachePolicy
{
    /// <summary>
    /// Gets or sets a value indicating whether caching is enabled.
    /// </summary>
    public bool IsCachingEnabled { get; set; } = true;
    /// Gets or sets the cache duration.
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(5);
    /// Gets or sets the cache priority.
    public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;
    /// Gets or sets a function to determine if a response should be cached.
    public Func<object, bool> ShouldCache { get; set; } = _ => true;
    /// Gets or sets a function to determine if a cached response should be refreshed.
    public Func<object, bool> ShouldRefresh { get; set; } = _ => false;
}
