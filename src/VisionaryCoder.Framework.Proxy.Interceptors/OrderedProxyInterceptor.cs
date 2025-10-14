using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Abstractions.Interceptors;

namespace VisionaryCoder.Framework.Proxy.Interceptors;

public sealed class OrderedProxyInterceptor<TInner>(TInner inner, int order) : IProxyInterceptor, IOrderedProxyInterceptor where TInner : IProxyInterceptor
{
    public int Order => order;
    public Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next) => inner.InvokeAsync(context, next);
}
