namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Interface for audit sinks that persist audit records.
/// </summary>
public interface IAuditSink
{
    /// <summary>
    /// Writes an audit record asynchronously.
    /// </summary>
    /// <param name="record">The audit record to write.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteAsync(AuditRecord record);

    /// <summary>
    /// Writes multiple audit records asynchronously.
    /// </summary>
    /// <param name="records">The audit records to write.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteBatchAsync(IEnumerable<AuditRecord> records);
}