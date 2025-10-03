namespace VisionaryCoder.Proxy.Interceptors;

public sealed class LoggingInterceptor(ILogger<LoggingInterceptor> logger) : IProxyInterceptor
{
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        logger.LogDebug("Proxy request {Type} Correlation={CorrelationId}", context.Request.GetType().Name, context.CorrelationId);
        var response = await next(context);
        if (response.IsSuccess)
            logger.LogInformation("Proxy success {ResultType} Correlation={CorrelationId}", typeof(T).Name, context.CorrelationId);
        else
            logger.LogWarning(response.Error!, "Proxy failure {ResultType} Correlation={CorrelationId}", typeof(T).Name, context.CorrelationId);
        return response;
    }
}