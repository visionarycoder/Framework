// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Abstractions.Interceptors;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Resilience.Abstractions;

/// <summary>
/// Null object pattern implementation of resilience interceptor that performs no operations.
/// </summary>
public sealed class NullResilienceInterceptor : IOrderedProxyInterceptor
{
    /// <inheritdoc />
    public int Order => 180;

    /// <inheritdoc />
    public Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        // Pass through without any resilience patterns
        return next();
    }
}
