namespace VisionaryCoder.Proxy.Abstractions;

public interface IProxyClient
{
    Task<Response<T>> SendAsync<T>(object request, CancellationToken cancellationToken = default);
}