// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Caching.Providers;
using VisionaryCoder.Framework.Proxy;

namespace VisionaryCoder.Framework.Caching.Interceptors;

/// <summary>
/// Interceptor that provides intelligent caching for proxy operations to improve performance.
/// Uses configurable cache policies and providers for flexible caching strategies.
/// </summary>
public sealed class CachingInterceptor(
    ILogger<CachingInterceptor> logger,
    IProxyCache proxyCache,
    ICacheKeyProvider keyProvider,
    ICachePolicyProvider policyProvider) : IOrderedProxyInterceptor
{
    /// <inheritdoc />
    public int Order => 50; // Caching typically runs in the middle of the pipeline

    private readonly ILogger<CachingInterceptor> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IProxyCache proxyCache = proxyCache ?? throw new ArgumentNullException(nameof(proxyCache));
    private readonly ICacheKeyProvider keyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
    private readonly ICachePolicyProvider policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));

    /// <summary>
    /// Invokes the caching interceptor logic for proxy operations.
    /// Attempts cache retrieval before execution and caches successful responses.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    /// <param name="context">The proxy context containing request information.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task representing the async operation with the response.</returns>
    public async Task<ProxyResponse<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next, CancellationToken cancellationToken = default)
    {
        var operationName = context.OperationName ?? "Unknown";
        var correlationId = context.CorrelationId ?? "None";

        // Check if caching is explicitly disabled for this request
        if (IsCachingDisabled(context))
        {
            logger.LogDebug("Caching disabled for operation '{OperationName}'. Correlation ID: '{CorrelationId}'",
                operationName, correlationId);
            return await next(context, cancellationToken);
        }

        // Get cache policy to determine if we should cache this operation
        var cachePolicy = policyProvider.GetPolicy(context);
        if (!cachePolicy.IsCachingEnabled || !policyProvider.ShouldCache(context))
        {
            logger.LogDebug("Caching policy disabled for operation '{OperationName}'. Correlation ID: '{CorrelationId}'",
                operationName, correlationId);
            return await next(context, cancellationToken);
        }

        // Generate and try to retrieve from cache
        var cacheKey = keyProvider.GenerateKey(context);
        if (cacheKey == null)
        {
            logger.LogDebug("No cache key generated for operation '{OperationName}', bypassing cache. Correlation ID: '{CorrelationId}'", 
                operationName, correlationId);
            return await next(context, cancellationToken);
        }
        
        var cachedResponse = await proxyCache.GetAsync<T>(cacheKey, cancellationToken);

        if (cachedResponse != null)
        {
            logger.LogDebug("Cache hit for operation '{OperationName}' with key '{CacheKey}'. Correlation ID: '{CorrelationId}'",
                operationName, cacheKey, correlationId);
            
            context.Metadata["CacheHit"] = true;
            context.Metadata["CacheKey"] = cacheKey;
            
            return cachedResponse;
        }

        // Cache miss - execute the operation
        logger.LogDebug("Cache miss for operation '{OperationName}' with key '{CacheKey}'. Correlation ID: '{CorrelationId}'",
            operationName, cacheKey, correlationId);

        var response = await next(context, cancellationToken);

        // Cache successful responses based on policy
        if (ShouldCacheResponse(response, cachePolicy))
        {
            var expiration = policyProvider.GetExpiration(context) ?? cachePolicy.Duration;
            
            await proxyCache.SetAsync(cacheKey, response, expiration, cancellationToken);
            
            logger.LogDebug("Cached successful response for operation '{OperationName}' with key '{CacheKey}' for {Duration}. Correlation ID: '{CorrelationId}'",
                operationName, cacheKey, expiration, correlationId);
        }

        context.Metadata["CacheHit"] = false;
        context.Metadata["CacheKey"] = cacheKey;
        
        return response;
    }

    /// <summary>
    /// Determines if caching is explicitly disabled for the current request.
    /// </summary>
    /// <param name="context">The proxy context to evaluate.</param>
    /// <returns>True if caching should be disabled for this request.</returns>
    private static bool IsCachingDisabled(ProxyContext context)
    {
        // Check for explicit cache disable flag
        if (context.Metadata.TryGetValue("DisableCache", out var disableCache) &&
            disableCache is bool disabled && disabled)
        {
            return true;
        }

        // Check for cache-control headers that indicate no-cache
        if (context.Headers.TryGetValue("Cache-Control", out var cacheControl))
        {
            var cacheControlValue = cacheControl?.ToString()?.ToLowerInvariant();
            if (cacheControlValue?.Contains("no-cache") == true || 
                cacheControlValue?.Contains("no-store") == true)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines if a response should be cached based on the response and cache policy.
    /// </summary>
    /// <typeparam name="T">The response type.</typeparam>
    /// <param name="response">The response to evaluate.</param>
    /// <param name="policy">The cache policy to apply.</param>
    /// <returns>True if the response should be cached.</returns>
    private static bool ShouldCacheResponse<T>(ProxyResponse<T> response, CachePolicy policy)
    {
        // Only cache successful responses
        if (!response.IsSuccess)
        {
            return false;
        }

        // Apply policy-specific caching decision
        if (policy.ShouldCache != null && response.Data != null)
        {
            return policy.ShouldCache(response.Data);
        }

        return true;
    }
}