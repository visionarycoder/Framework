// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Correlation.Abstractions;

/// <summary>
/// Null object pattern implementation of correlation interceptor that performs no operations.
/// </summary>
public sealed class NullCorrelationInterceptor : IOrderedProxyInterceptor
{
    /// <inheritdoc />
    public int Order => 0;

    /// <inheritdoc />
    public Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next, CancellationToken cancellationToken = default)
    {
        // Pass through without any correlation processing
        return next(context, cancellationToken);
    }
}