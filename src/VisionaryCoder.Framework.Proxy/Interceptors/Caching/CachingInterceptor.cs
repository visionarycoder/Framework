// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Proxy.Abstractions;
namespace VisionaryCoder.Framework.Proxy.Interceptors.Caching;
/// <summary>
/// Interceptor that provides caching for proxy operations to improve performance.
/// </summary>
public sealed class CachingInterceptor : IOrderedProxyInterceptor
{
    /// <inheritdoc />
    public int Order => 50; // Caching typically runs in the middle of the pipeline
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
    /// Invokes the interceptor with caching logic for the proxy operation.
    /// <typeparam name="T">The type of the response data.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation with the response.</returns>
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next, CancellationToken cancellationToken = default)
    {
        string operationName = context.OperationName ?? "Unknown";
        string correlationId = context.CorrelationId ?? "None";

        // Check if caching is disabled for this operation
        if (context.Metadata.TryGetValue("DisableCache", out object? disableCache) &&
            disableCache is bool disabled && disabled)
        {
            logger.LogDebug("Caching disabled for operation '{OperationName}'. Correlation ID: '{CorrelationId}'",
                operationName, correlationId);
            return await next(context, cancellationToken);
        }

        // Generate cache key
        string cacheKey = GenerateCacheKey(context);

        // Try to get from cache first
        if (cache.TryGetValue(cacheKey, out object? cachedResponse) && cachedResponse is Response<T> cached)
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
        Response<T> response = await next(context, cancellationToken);

        // Cache successful responses only
        if (response.IsSuccess)
        {
            TimeSpan cacheDuration = GetCacheDuration(context);
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
        foreach (KeyValuePair<string, object?> kvp in context.Metadata.Where(m => IsRelevantForCaching(m.Key)))
        {
            keyParts.Add($"{kvp.Key}:{kvp.Value}");
        }

        return string.Join("|", keyParts);
    }

    private TimeSpan GetCacheDuration(ProxyContext context)
    {
        if (context.Metadata.TryGetValue("CacheDurationSeconds", out object? durationObj) &&
            durationObj is int seconds && seconds > 0)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        return options.DefaultDuration;
    }

    private static bool IsRelevantForCaching(string metadataKey)
    {
        // Exclude non-relevant keys from cache key generation
        string[] excludeKeys = new[]
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
