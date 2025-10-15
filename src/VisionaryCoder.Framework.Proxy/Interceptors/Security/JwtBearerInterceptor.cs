// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Interceptor that adds JWT Bearer authentication to proxy operations.
/// </summary>
public sealed class JwtBearerInterceptor : IProxyInterceptor
{
    private readonly ILogger<JwtBearerInterceptor> logger;
    private readonly Func<Task<string?>> tokenProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtBearerInterceptor"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="tokenProvider">Function that provides the JWT token.</param>
    public JwtBearerInterceptor(ILogger<JwtBearerInterceptor> logger, Func<Task<string?>> tokenProvider)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
    }

    /// <summary>
    /// Invokes the interceptor adding JWT Bearer authentication to the context.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation with the response.</returns>
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        var operationName = context.OperationName ?? "Unknown";
        var correlationId = context.CorrelationId ?? "None";

        try
        {
            // Get the JWT token
            var token = await tokenProvider();
            
            if (string.IsNullOrEmpty(token))
            {
                logger.LogWarning("No JWT token available for operation '{OperationName}'. Correlation ID: '{CorrelationId}'", 
                    operationName, correlationId);
                
                throw new TransientProxyException($"Authentication failed: No JWT token available for operation '{operationName}'");
            }

            // Add the Authorization header to the context
            if (!context.Metadata.ContainsKey("Authorization"))
            {
                context.Metadata["Authorization"] = $"Bearer {token}";
                
                logger.LogDebug("Added JWT Bearer token to operation '{OperationName}'. Correlation ID: '{CorrelationId}'", 
                    operationName, correlationId);
            }

            return await next(context);
        }
        catch (Exception ex) when (!(ex is ProxyException))
        {
            logger.LogError(ex, "Authentication failed for operation '{OperationName}'. Correlation ID: '{CorrelationId}'", 
                operationName, correlationId);
            
            throw new TransientProxyException($"Authentication failed for operation '{operationName}': {ex.Message}", ex);
        }
    }
}

