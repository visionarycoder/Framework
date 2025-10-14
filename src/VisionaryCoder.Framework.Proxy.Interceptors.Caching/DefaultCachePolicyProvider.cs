using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Caching;

/// <summary>
/// Default implementation of ICachePolicyProvider.
/// </summary>
public class DefaultCachePolicyProvider : ICachePolicyProvider
{
    private readonly CachingOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultCachePolicyProvider"/> class.
    /// </summary>
    /// <param name="options">The caching options.</param>
    public DefaultCachePolicyProvider(CachingOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Gets the cache policy based on the operation and method.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>The cache policy to apply.</returns>
    public CachePolicy GetPolicy(ProxyContext context)
    {
        // Only cache GET operations by default
        if (!string.Equals(context.Method, "GET", StringComparison.OrdinalIgnoreCase))
        {
            return new CachePolicy { IsCachingEnabled = false };
        }

        // Check for specific operation policies
        if (options.OperationPolicies.TryGetValue(context.OperationName ?? string.Empty, out var policy))
        {
            return policy;
        }

        // Return default policy
        return new CachePolicy
        {
            Duration = options.DefaultDuration,
            Priority = options.DefaultPriority
        };
    }
}