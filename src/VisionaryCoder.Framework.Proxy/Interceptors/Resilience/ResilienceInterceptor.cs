// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Logging;
using Polly;
using VisionaryCoder.Framework.Proxy.Abstractions;
namespace VisionaryCoder.Framework.Proxy.Interceptors.Resilience;
/// <summary>
/// Interceptor that provides resilience capabilities using Microsoft.Extensions.Resilience and Polly.
/// </summary>
public sealed class ResilienceInterceptor : IOrderedProxyInterceptor
{
    private readonly ILogger<ResilienceInterceptor> logger;
    private readonly ResiliencePipeline resiliencePipeline;
    /// <summary>
    /// Gets the execution order for this interceptor.
    /// </summary>
    public int Order => 10; // Resilience typically runs early in the pipeline
    /// Initializes a new instance of the <see cref="ResilienceInterceptor"/> class.
    /// <param name="logger">The logger instance.</param>
    /// <param name="resiliencePipeline">The configured resilience pipeline.</param>
    public ResilienceInterceptor(ILogger<ResilienceInterceptor> logger, ResiliencePipeline? resiliencePipeline = null)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.resiliencePipeline = resiliencePipeline ?? CreateDefaultPipeline();
    }
    /// Invokes the interceptor with resilience protection.
    /// <typeparam name="T">The type of the response data.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation with the response.</returns>
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next, CancellationToken cancellationToken = default)
    {
        string operationName = context.OperationName ?? "Unknown";
        string correlationId = context.CorrelationId ?? "Undefined";
        try
        {
            logger.LogDebug("Applying resilience pipeline for operation '{OperationName}'. Correlation ID: '{CorrelationId}'", operationName, correlationId);
            Response<T> response = await resiliencePipeline.ExecuteAsync(async (ct) => await next(context, ct), cancellationToken);
            context.Metadata["ResilienceApplied"] = "true";
            logger.LogDebug("Resilience pipeline completed successfully for operation '{OperationName}'. Correlation ID: '{CorrelationId}'", operationName, correlationId);
            return response;
        }
        catch (Exception ex)
        {
            context.Metadata["ResilienceException"] = ex.GetType().Name;
            logger.LogError(ex, "Resilience pipeline failed for operation '{OperationName}'. Correlation ID: '{CorrelationId}'", operationName, correlationId);
            throw;
        }
    }
    /// Creates a default resilience pipeline with retry and circuit breaker.
    /// <returns>A configured resilience pipeline.</returns>
    private static ResiliencePipeline CreateDefaultPipeline()
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new()
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true
            })
            .AddCircuitBreaker(new()
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(30),
                MinimumThroughput = 5,
                BreakDuration = TimeSpan.FromMinutes(1)
            })
            .AddTimeout(TimeSpan.FromSeconds(30))
            .Build();
    }
}
