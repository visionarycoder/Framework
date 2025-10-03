using VisionaryCoder.Proxy.Abstractions;

namespace VisionaryCoder.Proxy.Interceptors;

public sealed class RetryInterceptor(ProxyOptions options) : IProxyInterceptor
{
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        var attempt = 0;
        while (true)
        {
            var result = await next(context);
            if (result.IsSuccess) return result;

            if (result.Error is not RetryableTransportException || attempt >= options.MaxRetries)
                return result;

            attempt++;
            var delay = TimeSpan.FromMilliseconds(options.RetryDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
            await Task.Delay(delay, context.CancellationToken);
        }
    }
}