// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Logging;
using System.Diagnostics;
using VisionaryCoder.Framework.Proxy.Abstractions;
namespace VisionaryCoder.Framework.Proxy.Interceptors;
/// <summary>
/// Interceptor that measures and logs the execution time of proxy operations.
/// </summary>
public sealed class TimingInterceptor(ILogger<TimingInterceptor> logger) : IProxyInterceptor
{
    private readonly ILogger<TimingInterceptor> logger = logger;
    /// <summary>
    /// Invokes the interceptor with timing measurement of the proxy operation.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation with the response.</returns>
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next, CancellationToken cancellationToken = default)
    {
        var operationName = context.OperationName ?? "Unknown";
        var correlationId = context.CorrelationId ?? "None";
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await next(context, cancellationToken);
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            
            // Store timing in context metadata for other interceptors
            context.Metadata["ExecutionTimeMs"] = elapsedMs;
            if (elapsedMs > 1000) // Log warning if operation takes more than 1 second
            {
                logger.LogWarning("Slow proxy operation '{OperationName}' completed in {ElapsedMs}ms. Correlation ID: '{CorrelationId}'", 
                    operationName, elapsedMs, correlationId);
            }
            else
            {
                logger.LogDebug("Proxy operation '{OperationName}' completed in {ElapsedMs}ms. Correlation ID: '{CorrelationId}'", 
                    operationName, elapsedMs, correlationId);
            }
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "Proxy operation '{OperationName}' failed after {ElapsedMs}ms. Correlation ID: '{CorrelationId}'", 
                operationName, stopwatch.ElapsedMilliseconds, correlationId);
            throw;
        }
    }
}
