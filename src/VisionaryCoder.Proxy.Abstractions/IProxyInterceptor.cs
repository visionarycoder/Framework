namespace VisionaryCoder.Proxy.Abstractions;

public interface IProxyInterceptor
{
    // Interceptor can inspect/modify context and decide when to call next.
    Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next);
}