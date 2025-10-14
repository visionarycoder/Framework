// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Abstractions.Interceptors;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Auditing;

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
/// Auditing interceptor that emits audit records for proxy operations.
/// Order: 300 (executes last in the pipeline).
/// </summary>
public sealed class AuditingInterceptor : IOrderedProxyInterceptor
{
    private readonly ILogger<AuditingInterceptor> logger;
    private readonly IEnumerable<IAuditSink> auditSinks;

    /// <inheritdoc />
    public int Order => 300;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditingInterceptor"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="auditSinks">The audit sinks.</param>
    public AuditingInterceptor(ILogger<AuditingInterceptor> logger, IEnumerable<IAuditSink> auditSinks)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.auditSinks = auditSinks ?? throw new ArgumentNullException(nameof(auditSinks));
    }

    /// <inheritdoc />
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        var requestType = context.Request?.GetType().Name ?? "Unknown";
        var correlationId = context.Items.TryGetValue("CorrelationId", out var corrId) ? 
            corrId?.ToString() ?? Guid.NewGuid().ToString("D") : 
            Guid.NewGuid().ToString("D");
        
        var startTime = DateTime.UtcNow;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = await next();
            
            stopwatch.Stop();
            
            // Create audit record
            var auditRecord = new AuditRecord(
                CorrelationId: correlationId,
                Operation: $"Proxy.{requestType}",
                RequestType: requestType,
                Timestamp: startTime,
                Success: result.IsSuccess,
                Error: result.IsSuccess ? null : result.Error,
                Duration: stopwatch.Elapsed,
                Metadata: CreateMetadata(context, result)
            );

            // Emit to all audit sinks
            await EmitAuditRecord(auditRecord);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Create audit record for exception
            var auditRecord = new AuditRecord(
                CorrelationId: correlationId,
                Operation: $"Proxy.{requestType}",
                RequestType: requestType,
                Timestamp: startTime,
                Success: false,
                Error: ex.Message,
                Duration: stopwatch.Elapsed,
                Metadata: CreateMetadata(context, null, ex)
            );

            // Emit to all audit sinks (best effort, don't let audit failure affect the operation)
            try
            {
                await EmitAuditRecord(auditRecord);
            }
            catch (Exception auditEx)
            {
                logger.LogWarning(auditEx, "Failed to emit audit record for failed operation");
            }
            
            throw;
        }
    }

    private async Task EmitAuditRecord(AuditRecord auditRecord)
    {
        foreach (var sink in auditSinks)
        {
            try
            {
                await sink.EmitAsync(auditRecord, CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to emit audit record to sink {SinkType}", sink.GetType().Name);
            }
        }
    }

    private static Dictionary<string, object?> CreateMetadata<T>(
        ProxyContext context, 
        Response<T>? result = null, 
        Exception? exception = null)
    {
        var metadata = new Dictionary<string, object?>
        {
            ["ResultType"] = context.ResultType.Name
        };

        // Add context items (excluding sensitive data)
        foreach (var item in context.Items.Where(kvp => !IsSensitiveKey(kvp.Key)))
        {
            metadata[$"Context.{item.Key}"] = item.Value;
        }

        if (exception != null)
        {
            metadata["Exception.Type"] = exception.GetType().Name;
            metadata["Exception.StackTrace"] = exception.StackTrace;
        }

        return metadata;
    }

    private static bool IsSensitiveKey(string key)
    {
        var sensitiveKeys = new[] { "Authorization", "Password", "Secret", "Token", "Key" };
        return sensitiveKeys.Any(sensitive => 
            key.Contains(sensitive, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Default audit sink that logs audit records.
/// </summary>
public sealed class LoggingAuditSink : IAuditSink
{
    private readonly ILogger<LoggingAuditSink> logger;

    public LoggingAuditSink(ILogger<LoggingAuditSink> logger)
    {
        logger = logger;
    }

    public Task EmitAsync(AuditRecord auditRecord, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Audit: {Operation} | Success: {Success} | Duration: {Duration}ms | CorrelationId: {CorrelationId}",
            auditRecord.Operation,
            auditRecord.Success,
            auditRecord.Duration?.TotalMilliseconds ?? 0,
            auditRecord.CorrelationId);

        return Task.CompletedTask;
    }
}
