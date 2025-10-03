using VisionaryCoder.Proxy.Abstractions;

namespace VisionaryCoder.Proxy.Interceptors;

public sealed class CachingInterceptor(
    IProxyCache cache,
    ICacheKeyProvider keyProvider,
    ICachePolicyProvider policyProvider) : IProxyInterceptor
{
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        if (!policyProvider.IsCacheable(context.Request, typeof(T)))
            return await next(context);

        var key = keyProvider.GetKey(context.Request, typeof(T));
        if (string.IsNullOrWhiteSpace(key))
            return await next(context);

        if (cache.TryGetValue(key!, out T hit))
            return Response<T>.Success(hit) with { CorrelationId = context.CorrelationId };

        var response = await next(context);
        if (response.IsSuccess)
        {
            var ttl = policyProvider.GetTtl(context.Request, typeof(T)) ?? TimeSpan.FromMinutes(1);
            cache.Set(key!, response.Value!, ttl);
        }

        return response;
    }
}