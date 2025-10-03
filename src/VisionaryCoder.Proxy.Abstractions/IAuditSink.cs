namespace VisionaryCoder.Proxy.Abstractions;

public interface IAuditSink
{
    Task WriteAsync(AuditRecord record, CancellationToken cancellationToken = default);
}