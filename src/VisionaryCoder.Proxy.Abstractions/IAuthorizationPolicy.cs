namespace VisionaryCoder.Proxy.Abstractions;

public interface IAuthorizationPolicy
{
    Task<bool> AuthorizeAsync(ProxyContext context, CancellationToken cancellationToken = default);
}