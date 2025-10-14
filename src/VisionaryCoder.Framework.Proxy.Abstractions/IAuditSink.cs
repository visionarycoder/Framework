namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Defines a contract for audit sinks.
/// </summary>
public interface IAuditSink
{
    /// <summary>
    /// Writes an audit record.
    /// </summary>
    /// <param name="auditRecord">The audit record to write.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteAsync(AuditRecord auditRecord);
}