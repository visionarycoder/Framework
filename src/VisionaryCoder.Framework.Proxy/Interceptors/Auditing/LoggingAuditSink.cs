using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Auditing;

/// <summary>
/// Default audit sink that logs audit records.
/// </summary>
/// <param name="logger">The logger instance.</param>
public sealed class LoggingAuditSink(ILogger<LoggingAuditSink> logger) : IAuditSink
{
    private readonly ILogger<LoggingAuditSink> logger = logger;

    public Task WriteAsync(AuditRecord record)
    {
        logger.LogInformation("Audit: {OperationId} | Method: {MethodName} | Success: {Success} | Duration: {Duration}ms | Timestamp: {Timestamp}", 
                            record.OperationId, record.MethodName, record.Success, record.Duration.TotalMilliseconds, record.Timestamp);

        return Task.CompletedTask;
    }
}