using Microsoft.Extensions.Caching.Memory;
using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Caching;

/// <summary>
/// Configuration options for the caching interceptor.
/// </summary>
public sealed class CachingOptions
{
    /// <summary>
    /// Gets or sets the default cache duration.
    /// </summary>
    public TimeSpan DefaultDuration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the default cache priority.
    /// </summary>
    public CacheItemPriority DefaultPriority { get; set; } = CacheItemPriority.Normal;

    /// <summary>
    /// Gets or sets a value indicating whether to enable eviction logging.
    /// </summary>
    public bool EnableEvictionLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets operation-specific cache policies.
    /// </summary>
    public Dictionary<string, CachePolicy> OperationPolicies { get; set; } = new();

    /// <summary>
    /// The maximum size of the cache in entries.
    /// </summary>
    public int? MaxCacheSize { get; set; }

    /// <summary>
    /// Custom cache key generator function.
    /// </summary>
    public Func<ProxyContext, string>? KeyGenerator { get; set; }

    /// <summary>
    /// Predicate to determine if a response should be cached based on context.
    /// </summary>
    public Func<ProxyContext, bool>? ShouldCache { get; set; }
}