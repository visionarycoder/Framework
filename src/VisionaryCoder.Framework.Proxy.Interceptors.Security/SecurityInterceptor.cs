// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Abstractions.Exceptions;
using VisionaryCoder.Framework.Proxy.Abstractions.Interceptors;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Security interceptor that handles authentication and authorization for proxy operations.
/// Order: -200 (executes early in the pipeline).
/// </summary>
public sealed class SecurityInterceptor : IOrderedProxyInterceptor
{
    private readonly ILogger<SecurityInterceptor> logger;
    private readonly IEnumerable<IProxySecurityEnricher> enrichers;
    private readonly IEnumerable<IProxyAuthorizationPolicy> policies;

    /// <inheritdoc />
    public int Order => -200;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityInterceptor"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="enrichers">The security enrichers.</param>
    /// <param name="policies">The authorization policies.</param>
    public SecurityInterceptor(
        ILogger<SecurityInterceptor> logger,
        IEnumerable<IProxySecurityEnricher> enrichers,
        IEnumerable<IProxyAuthorizationPolicy> policies)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.enrichers = enrichers ?? throw new ArgumentNullException(nameof(enrichers));
        this.policies = policies ?? throw new ArgumentNullException(nameof(policies));
    }

    /// <inheritdoc />
    public async Task<Response<T>> InvokeAsync<T>(
        ProxyContext context,
        ProxyDelegate<T> next)
    {
        using var _ = logger.BeginScope("SecurityInterceptor for {RequestType}", context.Request?.GetType().Name ?? "Unknown");
        
        try
        {
            // Enrich security context
            foreach (var enricher in enrichers)
            {
                await enricher.EnrichAsync(context, CancellationToken.None);
            }

            // Check authorization policies
            foreach (var policy in policies)
            {
                if (!await policy.IsAuthorizedAsync(context, CancellationToken.None))
                {
                    logger.LogWarning("Authorization failed for policy {PolicyType}", policy.GetType().Name);
                    return Response<T>.Failure(new NonRetryableTransportException("Authorization failed"));
                }
            }

            logger.LogDebug("Security validation passed, proceeding to next interceptor");
            return await next(context);
        }
        catch (Exception ex) when (ex is not ProxyException)
        {
            logger.LogError(ex, "Unexpected error during security processing");
            return Response<T>.Failure(new NonRetryableTransportException("Security processing failed", ex));
        }
    }
}

