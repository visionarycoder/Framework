namespace VisionaryCoder.Proxy.Abstractions;

public sealed class AuditRecord(
    string action, string outcome, string? correlationId, string requestType, string resultType,
    DateTimeOffset timestamp, TimeSpan? duration, string? details = null)
{
    public string Action { get; } = action;
    public string Outcome { get; } = outcome; // Success|Failure
    public string? CorrelationId { get; } = correlationId;
    public string RequestType { get; } = requestType;
    public string ResultType { get; } = resultType;
    public DateTimeOffset Timestamp { get; } = timestamp;
    public TimeSpan? Duration { get; } = duration;
    public string? Details { get; } = details;
}