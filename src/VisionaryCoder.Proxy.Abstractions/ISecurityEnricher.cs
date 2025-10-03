namespace VisionaryCoder.Proxy.Abstractions;

public interface ISecurityEnricher
{
    Task EnrichAsync(ProxyContext context, CancellationToken cancellationToken = default);
}