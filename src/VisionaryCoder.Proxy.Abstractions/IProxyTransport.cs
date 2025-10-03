namespace VisionaryCoder.Proxy.Abstractions;

public interface IProxyTransport
{
    Task<Response<T>> SendAsync<T>(ProxyContext context);
}