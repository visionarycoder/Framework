using System.Diagnostics;
using VisionaryCoder.Proxy.Abstractions;

namespace VisionaryCoder.Proxy.Interceptors;

public sealed class TelemetryInterceptor(ActivitySource activitySource) : IProxyInterceptor
{
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        using var activity = activitySource.StartActivity("proxy.call", ActivityKind.Client);
        activity?.SetTag("vc.proxy.request_type", context.Request.GetType().FullName);
        activity?.SetTag("vc.proxy.result_type", context.ResultType.FullName);
        activity?.SetTag("vc.proxy.correlation_id", context.CorrelationId);

        var sw = Stopwatch.StartNew();
        var response = await next(context);
        sw.Stop();

        activity?.SetTag("vc.proxy.duration_ms", sw.ElapsedMilliseconds);
        if (!response.IsSuccess && response.Error is not null)
        {
            activity?.SetStatus(ActivityStatusCode.Error, response.Error.Message);
            activity?.RecordException(response.Error);
        }

        return response with { Duration = response.Duration ?? sw.Elapsed };
    }
}