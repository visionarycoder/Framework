using VisionaryCoder.Proxy.Abstractions;

namespace VisionaryCoder.Proxy.Interceptors;

public sealed class SecurityInterceptor(ISecurityEnricher enricher, IAuthorizationPolicy policy) : IProxyInterceptor
{
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        try
        {
            await enricher.EnrichAsync(context, context.CancellationToken);

            var allowed = await policy.AuthorizeAsync(context, context.CancellationToken);
            if (!allowed)
                return Response<T>.Failure(new BusinessException("Unauthorized proxy request"));

            return await next(context);
        }
        catch (ProxyException pe)
        {
            return Response<T>.Failure(pe);
        }
        catch (Exception ex)
        {
            return Response<T>.Failure(new NonRetryableTransportException("Security processing failed", ex));
        }
    }
}