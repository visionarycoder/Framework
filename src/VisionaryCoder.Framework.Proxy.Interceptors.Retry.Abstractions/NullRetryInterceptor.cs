// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Abstractions.Interceptors;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Retry.Abstractions;

/// <summary>
/// Null object pattern implementation of retry interceptor that performs no operations.
/// </summary>
public sealed class NullRetryInterceptor : IOrderedProxyInterceptor
{
    /// <inheritdoc />
    public int Order => 200;

    /// <inheritdoc />
    public Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        // Pass through without any retry logic
        return next();
    }
}
