// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Represents a proxy context containing metadata about the proxy operation.
/// </summary>
public class ProxyContext
{
    /// <summary>
    /// Gets or sets the operation identifier.
    /// </summary>
    public string OperationId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the method name being proxied.
    /// </summary>
    public string? MethodName { get; set; }

    /// <summary>
    /// Gets or sets the service name.
    /// </summary>
    public string? ServiceName { get; set; }

    /// <summary>
    /// Gets or sets additional properties for the operation.
    /// </summary>
    public Dictionary<string, object?> Properties { get; set; } = new();

    /// <summary>
    /// Gets or sets the correlation identifier.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the start time of the operation.
    /// </summary>
    public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the HTTP method.
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// Gets or sets the request URL.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the request headers.
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Gets or sets the request object.
    /// </summary>
    public object? Request { get; set; }

    /// <summary>
    /// Gets or sets additional items for the context.
    /// </summary>
    public Dictionary<string, object?> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets metadata for the operation.
    /// </summary>
    public Dictionary<string, object?> Metadata { get; set; } = new();

    /// <summary>
    /// Gets or sets the operation name.
    /// </summary>
    public string? OperationName { get; set; }

    /// <summary>
    /// Gets or sets the result type.
    /// </summary>
    public Type? ResultType { get; set; }

    /// <summary>
    /// Gets or sets the request identifier.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets or sets the cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; set; }
}

/// <summary>
/// Represents a delegate for the next operation in the proxy pipeline.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
/// <param name="context">The proxy context.</param>
/// <returns>A task representing the asynchronous operation with the response.</returns>
public delegate Task<Response<T>> ProxyDelegate<T>(ProxyContext context);

/// <summary>
/// Defines a contract for proxy interceptors.
/// </summary>
public interface IProxyInterceptor
{
    /// <summary>
    /// Invokes the interceptor with the given context and next delegate.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation with the response.</returns>
    Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next);
}

/// <summary>
/// Defines a contract for ordered proxy interceptors.
/// </summary>
public interface IOrderedProxyInterceptor : IProxyInterceptor
{
    /// <summary>
    /// Gets the order in which this interceptor should be executed.
    /// Lower values execute first.
    /// </summary>
    int Order { get; }
}

/// <summary>
/// Configuration options for proxy operations.
/// </summary>
public class ProxyOptions
{
    /// <summary>
    /// Gets or sets the timeout for proxy operations.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the number of failures before circuit breaker opens.
    /// </summary>
    public int CircuitBreakerFailures { get; set; } = 5;

    /// <summary>
    /// Gets or sets the duration the circuit breaker stays open.
    /// </summary>
    public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the retry delay.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets whether caching is enabled.
    /// </summary>
    public bool CachingEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets whether auditing is enabled.
    /// </summary>
    public bool AuditingEnabled { get; set; } = true;
}