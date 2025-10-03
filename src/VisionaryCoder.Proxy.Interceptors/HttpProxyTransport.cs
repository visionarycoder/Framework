using Microsoft.Extensions.Logging;
using VisionaryCoder.Proxy.Abstractions;

namespace VisionaryCoder.Proxy.Interceptors;

public sealed class HttpProxyTransport(IProxyErrorClassifier classifier, HttpClient http) : IProxyTransport
{
    public async Task<Response<T>> SendCoreAsync<T>(ProxyContext context)
    {
        try
        {
            // Example only: serialize request, call remote, deserialize to T
            var result = default(T)!;
            return Response<T>.Success(result);
        }
        catch (ProxyException pe)
        {
            return Response<T>.Failure(pe);
        }
        catch (Exception ex)
        {
            var kind = classifier.Classify(ex);
            var normalized = kind switch
            {
                ProxyErrorClassification.Transient => new RetryableTransportException(ex.Message, ex),
                ProxyErrorClassification.NonTransient => new NonRetryableTransportException(ex.Message, ex),
                ProxyErrorClassification.Business => new BusinessException(ex.Message, ex),
                _ => new NonRetryableTransportException("Unhandled proxy exception", ex)
            };
            return Response<T>.Failure(normalized);
        }
    }
}

public sealed class LoggingInterceptor(ILogger<LoggingInterceptor> logger) : IProxyInterceptor
{
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        logger.LogDebug("Proxy request {RequestType} => {ResultType} Correlation={CorrelationId}", context.Request.GetType().Name, context.ResultType.Name, context.CorrelationId);

        var response = await next(context);

        if (response.IsSuccess)
            logger.LogInformation("Proxy success {ResultType} Correlation={CorrelationId} Duration={Duration}", typeof(T).Name, context.CorrelationId, response.Duration);
        else
            logger.LogWarning(response.Error!, "Proxy failure {ResultType} Correlation={CorrelationId} Duration={Duration}", typeof(T).Name, context.CorrelationId, response.Duration);
        return response;
    }
}