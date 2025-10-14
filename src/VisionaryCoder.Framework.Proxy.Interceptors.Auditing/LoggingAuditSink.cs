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

    public Task EmitAsync(AuditRecord auditRecord, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Audit: {Operation} | Success: {Success} | Duration: {Duration}ms | CorrelationId: {CorrelationId}", auditRecord.Operation, auditRecord.Success, auditRecord.Duration?.TotalMilliseconds ?? 0, auditRecord.CorrelationId);

        return Task.CompletedTask;
    }
}