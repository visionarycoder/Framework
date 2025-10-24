namespace VisionaryCoder.Framework.Proxy.Abstractions.Interceptors;

/// <summary>
/// Base interface for all proxy interceptors.
/// </summary>
public interface IInterceptor
{
    /// <summary>
    /// Gets the order in which this interceptor should be executed.
    /// Lower numbers execute first.
    /// </summary>
    int Order { get; }
}
/// Interface for caching interceptors.
public interface ICachingInterceptor : IInterceptor
{
    /// <summary>
    /// Intercepts method calls for caching.
    /// </summary>
    /// <param name="methodName">The name of the method being called.</param>
    /// <param name="parameters">The method parameters.</param>
    /// <param name="next">The next operation in the pipeline.</param>
    /// <returns>The result of the operation.</returns>
    Task<T> InterceptAsync<T>(string methodName, object[] parameters, Func<Task<T>> next);
}
/// Interface for logging interceptors.
public interface ILoggingInterceptor : IInterceptor
{
    /// <summary>
    /// Intercepts method calls for logging.
    /// </summary>
    /// <param name="methodName">The name of the method being called.</param>
    /// <param name="parameters">The method parameters.</param>
    /// <param name="next">The next operation in the pipeline.</param>
    /// <returns>The result of the operation.</returns>
    Task<T> InterceptAsync<T>(string methodName, object[] parameters, Func<Task<T>> next);
}

/// <summary>
/// Interface for telemetry interceptors.
/// </summary>
public interface ITelemetryInterceptor : IInterceptor
{
    /// <summary>
    /// Intercepts method calls for telemetry.
    /// </summary>
    /// <param name="methodName">The name of the method being called.</param>
    /// <param name="parameters">The method parameters.</param>
    /// <param name="next">The next operation in the pipeline.</param>
    /// <returns>The result of the operation.</returns>
    Task<T> InterceptAsync<T>(string methodName, object[] parameters, Func<Task<T>> next);
}

/// <summary>
/// Interface for retry interceptors.
/// </summary>
public interface IRetryInterceptor : IInterceptor
{
    /// <summary>
    /// Intercepts method calls for retry logic.
    /// </summary>
    /// <param name="methodName">The name of the method being called.</param>
    /// <param name="parameters">The method parameters.</param>
    /// <param name="next">The next operation in the pipeline.</param>
    /// <returns>The result of the operation.</returns>
    Task<T> InterceptAsync<T>(string methodName, object[] parameters, Func<Task<T>> next);
}
