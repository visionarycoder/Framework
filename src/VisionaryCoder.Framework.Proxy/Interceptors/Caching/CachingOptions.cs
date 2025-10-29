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
    /// Gets or sets the default cache priority.
    public CacheItemPriority DefaultPriority { get; set; } = CacheItemPriority.Normal;
    /// Gets or sets a value indicating whether to enable eviction logging.
    public bool EnableEvictionLogging { get; set; } = false;
    /// Gets or sets operation-specific cache policies.
    public Dictionary<string, CachePolicy> OperationPolicies { get; set; } = new();
    /// The maximum size of the cache in entries.
    public int? MaxCacheSize { get; set; }
    /// Custom cache key generator function.
    public Func<ProxyContext, string>? KeyGenerator { get; set; }
    /// Predicate to determine if a response should be cached based on context.
    public Func<ProxyContext, bool>? ShouldCache { get; set; }
}
