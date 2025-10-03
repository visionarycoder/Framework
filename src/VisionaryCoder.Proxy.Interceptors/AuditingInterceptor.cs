using VisionaryCoder.Proxy.Abstractions;

namespace VisionaryCoder.Proxy.Interceptors;

public sealed class AuditingInterceptor(IAuditSink sink) : IProxyInterceptor
{
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        var started = DateTimeOffset.UtcNow;
        var response = await next(context);

        var record = new AuditRecord(
            action: context.Request.GetType().Name,
            outcome: response.IsSuccess ? "Success" : "Failure",
            correlationId: context.CorrelationId,
            requestType: context.Request.GetType().FullName!,
            resultType: typeof(T).FullName!,
            timestamp: started,
            duration: response.Duration,
            details: response.IsSuccess ? null : response.Error?.Message
        );

        await sink.WriteAsync(record, context.CancellationToken);
        return response;
    }
}