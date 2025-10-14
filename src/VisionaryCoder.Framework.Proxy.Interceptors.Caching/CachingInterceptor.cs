// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Caching;

/// <summary>
/// Interceptor that provides caching for proxy operations to improve performance.
/// </summary>
public sealed class CachingInterceptor : IProxyInterceptor
{
    private readonly ILogger<CachingInterceptor> logger;
    private readonly IMemoryCache cache;
    private readonly CachingOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingInterceptor"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="options">The caching options.</param>
    public CachingInterceptor(
        ILogger<CachingInterceptor> logger, 
        IMemoryCache cache, 
        CachingOptions options)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Invokes the interceptor with caching logic for the proxy operation.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation with the response.</returns>
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        var operationName = context.OperationName ?? "Unknown";
        var correlationId = context.CorrelationId ?? "None";

        // Check if caching is disabled for this operation
        if (context.Metadata.TryGetValue("DisableCache", out var disableCache) && 
            disableCache is bool disabled && disabled)
        {
            logger.LogDebug("Caching disabled for operation '{OperationName}'. Correlation ID: '{CorrelationId}'", 
                operationName, correlationId);
            return await next(context);
        }

        // Generate cache key
        var cacheKey = GenerateCacheKey(context);
        
        // Try to get from cache first
        if (cache.TryGetValue(cacheKey, out var cachedResponse) && cachedResponse is Response<T> cached)
        {
            logger.LogDebug("Cache hit for operation '{OperationName}' with key '{CacheKey}'. Correlation ID: '{CorrelationId}'", 
                operationName, cacheKey, correlationId);
            
            context.Metadata["CacheHit"] = true;
            return cached;
        }

        // Execute the operation
        logger.LogDebug("Cache miss for operation '{OperationName}' with key '{CacheKey}'. Correlation ID: '{CorrelationId}'", 
            operationName, cacheKey, correlationId);

        // If cache miss, call next delegate to get the result
        var response = await next(context);
        
        // Cache successful responses only
        if (response.IsSuccess)
        {
            var cacheDuration = GetCacheDuration(context);
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration,
                Priority = CacheItemPriority.Normal
            };

            cache.Set(cacheKey, response, cacheEntryOptions);
            
            logger.LogDebug("Cached successful response for operation '{OperationName}' with key '{CacheKey}' for {Duration}. Correlation ID: '{CorrelationId}'", 
                operationName, cacheKey, cacheDuration, correlationId);
        }

        context.Metadata["CacheHit"] = false;
        return response;
    }

    private string GenerateCacheKey(ProxyContext context)
    {
        if (options.KeyGenerator != null)
        {
            return options.KeyGenerator(context);
        }

        // Default key generation based on operation name and metadata
        var keyParts = new List<string>
        {
            context.OperationName ?? "Unknown"
        };

        // Include relevant metadata in the key
        foreach (var kvp in context.Metadata.Where(m => IsRelevantForCaching(m.Key)))
        {
            keyParts.Add($"{kvp.Key}:{kvp.Value}");
        }

        return string.Join("|", keyParts);
    }

    private TimeSpan GetCacheDuration(ProxyContext context)
    {
        if (context.Metadata.TryGetValue("CacheDurationSeconds", out var durationObj) &&
            durationObj is int seconds && seconds > 0)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        return options.DefaultDuration;
    }

    private static bool IsRelevantForCaching(string metadataKey)
    {
        // Exclude non-relevant keys from cache key generation
        var excludeKeys = new[]
        {
            "CorrelationId",
            "ExecutionTimeMs",
            "RetryAttempts",
            "CircuitBreakerState",
            "CacheHit",
            "Authorization" // Sensitive data
        };

        return !excludeKeys.Contains(metadataKey, StringComparer.OrdinalIgnoreCase);
    }
}

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

/// <summary>
/// Default implementation of ICacheKeyProvider.
/// </summary>
public class DefaultCacheKeyProvider : ICacheKeyProvider
{
    /// <summary>
    /// Generates a cache key based on the operation name, URL, and method.
    /// </summary>
    /// <typeparam name="T">The response type.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <returns>A unique cache key.</returns>
    public string GenerateKey<T>(ProxyContext context)
    {
        var keyComponents = new List<string>
        {
            context.OperationName ?? "Unknown",
            context.Method,
            context.Url ?? string.Empty,
            typeof(T).Name
        };

        // Include relevant headers in the key
        if (context.Headers.Count > 0)
        {
            var headerString = string.Join(";", context.Headers
                .Where(h => IsRelevantHeader(h.Key))
                .OrderBy(h => h.Key)
                .Select(h => $"{h.Key}={h.Value}"));
            
            if (!string.IsNullOrEmpty(headerString))
            {
                keyComponents.Add(headerString);
            }
        }

        var combinedKey = string.Join("|", keyComponents);
        
        // Hash the key to ensure consistent length and avoid special characters
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(combinedKey));
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Determines if a header should be included in the cache key.
    /// </summary>
    /// <param name="headerName">The header name.</param>
    /// <returns>True if the header should be included.</returns>
    private static bool IsRelevantHeader(string headerName)
    {
        var relevantHeaders = new[]
        {
            "Accept",
            "Accept-Language",
            "Content-Type",
            "X-API-Version"
        };

        return relevantHeaders.Any(h => h.Equals(headerName, StringComparison.OrdinalIgnoreCase));
    }
}

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

