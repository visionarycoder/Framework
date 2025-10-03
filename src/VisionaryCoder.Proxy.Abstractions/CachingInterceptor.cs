namespace VisionaryCoder.Proxy.Abstractions;

[ProxyInterceptorOrder(50)]
public sealed class CachingInterceptor : IProxyInterceptor
{
    public Task<Response<T>> InvokeAsync<T>(ProxyContext ctx, ProxyDelegate<T> next) => next(ctx); 
}