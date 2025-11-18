// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace VisionaryCoder.Framework.Proxy.Interceptors;

/// <summary>
/// Defines a contract for logging interceptors that capture method call information.
/// Implementations should provide consistent logging behavior across proxy operations.
/// </summary>
public interface ILoggingInterceptor : IInterceptor
{
    /// <summary>
    /// Intercepts method calls for logging purposes, capturing method name, parameters, and execution results.
    /// </summary>
    /// <typeparam name="T">The return type of the intercepted method.</typeparam>
    /// <param name="methodName">The name of the method being intercepted.</param>
    /// <param name="parameters">The parameters passed to the method.</param>
    /// <param name="next">The next operation in the pipeline to execute.</param>
    /// <returns>The result of the operation wrapped in a task.</returns>
    Task<T> InterceptAsync<T>(string methodName, object[] parameters, Func<Task<T>> next);
}