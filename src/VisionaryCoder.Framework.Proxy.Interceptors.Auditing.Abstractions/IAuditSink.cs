namespace VisionaryCoder.Framework.Proxy.Interceptors.Auditing.Abstractions;

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