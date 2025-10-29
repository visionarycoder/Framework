using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Abstractions.Exceptions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Caching;

/// <summary>
/// Default implementation of <see cref="ICachePolicyProvider"/>.
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
    /// Gets the cache policy based on the operation and HTTP method.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>The cache policy to apply.</returns>
    public CachePolicy GetPolicy(ProxyContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Use the new interface methods for consistency
        if (!ShouldCache(context))
        {
            return new CachePolicy { IsCachingEnabled = false };
        }

        // Check for specific operation policies
        if (options.OperationPolicies.TryGetValue(context.OperationName ?? string.Empty, out CachePolicy? policy))
        {
            return policy;
        }

        // Return default policy
        return new CachePolicy
        {
            Duration = GetExpiration(context) ?? options.DefaultDuration,
            Priority = options.DefaultPriority
        };
    }

    /// <summary>
    /// Determines whether the request should be cached based on the context.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>True if the request should be cached; otherwise, false.</returns>
    public bool ShouldCache(ProxyContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Only cache GET operations by default
        if (!string.Equals(context.Method, "GET", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Check if specific operation has caching disabled
        if (options.OperationPolicies.TryGetValue(context.OperationName ?? string.Empty, out CachePolicy? policy))
        {
            return policy.IsCachingEnabled;
        }

        // Default to enabled for GET requests
        return true;
    }

    /// <summary>
    /// Gets the cache expiration duration for the given context.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>The cache expiration duration.</returns>
    public TimeSpan? GetExpiration(ProxyContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Check for specific operation policies
        if (options.OperationPolicies.TryGetValue(context.OperationName ?? string.Empty, out CachePolicy? policy))
        {
            return policy.Duration;
        }

        // Return default duration
        return options.DefaultDuration;
    }
}
