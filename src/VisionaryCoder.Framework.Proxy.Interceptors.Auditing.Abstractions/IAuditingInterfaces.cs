// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Abstractions.Interceptors;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Auditing.Abstractions;

/// <summary>
/// Represents an audit record for proxy operations.
/// </summary>
public sealed record AuditRecord(
    string CorrelationId,
    string Operation,
    string RequestType,
    DateTime Timestamp,
    bool Success,
    string? Error = null,
    TimeSpan? Duration = null,
    Dictionary<string, object?>? Metadata = null);

/// <summary>
/// Defines a contract for audit sinks that receive audit records.
/// </summary>
public interface IAuditSink
{
    /// <summary>
    /// Emits an audit record to the sink.
    /// </summary>
    /// <param name="auditRecord">The audit record to emit.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task EmitAsync(AuditRecord auditRecord, CancellationToken cancellationToken = default);
}

/// <summary>
/// Null object pattern implementation of auditing interceptor that performs no operations.
/// </summary>
public sealed class NullAuditingInterceptor : IOrderedProxyInterceptor
{
    /// <inheritdoc />
    public int Order => 300;

    /// <inheritdoc />
    public Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        // Pass through without any auditing
        return next();
    }
}