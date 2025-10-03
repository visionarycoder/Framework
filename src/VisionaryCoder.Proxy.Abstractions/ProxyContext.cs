namespace VisionaryCoder.Proxy.Abstractions;

public sealed class ProxyContext(object request, Type resultType, CancellationToken cancellationToken = default)
{
    public object Request { get; } = request;
    public Type ResultType { get; } = resultType;
    public string? CorrelationId { get; init; }
    public IDictionary<string, object> Items { get; } = new Dictionary<string, object>();
    public CancellationToken CancellationToken { get; } = cancellationToken;
}