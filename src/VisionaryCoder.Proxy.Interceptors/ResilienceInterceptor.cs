using System.Collections.Concurrent;
using VisionaryCoder.Proxy.Abstractions;

namespace VisionaryCoder.Proxy.Interceptors;

public sealed class ResilienceInterceptor(ProxyOptions options) : IProxyInterceptor
{
    private sealed class CircuitState(int failures, DateTimeOffset? openedAt)
    {
        public int Failures { get; set; } = failures;
        public DateTimeOffset? OpenedAt { get; set; } = openedAt;
    }

    private readonly ConcurrentDictionary<string, CircuitState> circuits = new();

    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        var key = $"{context.Request.GetType().FullName}->{context.ResultType.FullName}";
        var state = circuits.GetOrAdd(key, _ => new CircuitState(0, null));

        // Open -> short-circuit until duration elapses (then half-open)
        if (state.OpenedAt is DateTimeOffset opened &&
            opened.Add(options.CircuitBreakerDuration) > DateTimeOffset.UtcNow)
        {
            return Response<T>.Failure(new NonRetryableTransportException("Circuit is open"));
        }

        var cts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
        cts.CancelAfter(options.Timeout);

        var completed = await Task.WhenAny(next(context), Task.Delay(Timeout.InfiniteTimeSpan, cts.Token));
        if (completed is Task<Response<T>> task)
        {
            var response = await task;
            if (response.IsSuccess)
            {
                state.Failures = 0;
                state.OpenedAt = null;
                return response;
            }

            // count only non-business failures towards breaker
            if (response.Error is RetryableTransportException or NonRetryableTransportException)
            {
                state.Failures++;
                if (state.Failures >= options.CircuitBreakerFailures)
                    state.OpenedAt = DateTimeOffset.UtcNow;
            }

            return response;
        }

        // Timeout path
        state.Failures++;
        if (state.Failures >= options.CircuitBreakerFailures)
            state.OpenedAt = DateTimeOffset.UtcNow;

        return Response<T>.Failure(new RetryableTransportException($"Proxy timed out after {options.Timeout.TotalMilliseconds}ms"));
    }
}