// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Abstractions.Interceptors;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security.Abstractions;

/// <summary>
/// Defines a contract for enriching security context in proxy operations.
/// </summary>
public interface IProxySecurityEnricher
{
    /// <summary>
    /// Enriches the proxy context with security information.
    /// </summary>
    /// <param name="context">The proxy context to enrich.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task EnrichAsync(ProxyContext context, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a contract for authorization policies.
/// </summary>
public interface IProxyAuthorizationPolicy
{
    /// <summary>
    /// Determines whether the current context is authorized for the operation.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a boolean result indicating authorization status.</returns>
    Task<bool> IsAuthorizedAsync(ProxyContext context, CancellationToken cancellationToken = default);
}

/// <summary>
/// Null object pattern implementation of security interceptor that performs no operations.
/// </summary>
public sealed class NullSecurityInterceptor : IOrderedProxyInterceptor
{
    /// <inheritdoc />
    public int Order => -200;

    /// <inheritdoc />
    public Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        // Pass through without any security processing
        return next();
    }
}