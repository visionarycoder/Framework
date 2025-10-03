namespace VisionaryCoder.Proxy;

public sealed class DefaultProxyPipeline(IEnumerable<IProxyInterceptor> interceptors,
    IProxyTransport transport)
{
    public Task<Response<T>> SendAsync<T>(ProxyContext ctx)
    {
        var ordered = Order(interceptors);
        ProxyDelegate<T> terminal = c => transport.SendCoreAsync<T>(c);

        foreach (var interceptor in ordered.Reverse())
        {
            var next = terminal;
            terminal = c => interceptor.InvokeAsync<T>(c, next);
        }

        return terminal(ctx);
    }

    static IReadOnlyList<IProxyInterceptor> Order(IEnumerable<IProxyInterceptor> chain)
    {
        var i = 0;
        // DI preserves registration order—use index to keep stability for same order values
        return chain
            .Select(x => new
            {
                Interceptor = x,
                Order = GetOrder(x),
                Index = i++
            })
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Index)
            .Select(x => x.Interceptor)
            .ToList();
    }

    static int GetOrder(IProxyInterceptor interceptor)
    {
        if (interceptor is IOrderedProxyInterceptor o) return o.Order;

        var attr = interceptor.GetType().GetCustomAttribute<ProxyInterceptorOrderAttribute>();
        return attr?.Order ?? 0;
    }
}