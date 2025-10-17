// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Abstractions.Interceptors;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Caching.Abstractions;

/// <summary>
/// Defines a contract for proxy caching operations.
/// </summary>
public interface IProxyCache
{
    /// <summary>
    /// Gets a cached response for the given key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>The cached response, or null if not found.</returns>
    Task<Response<T>?> GetAsync<T>(string key);

    /// <summary>
    /// Sets a response in the cache with the given key and expiration.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="response">The response to cache.</param>
    /// <param name="expiration">The cache expiration time.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync<T>(string key, Response<T> response, TimeSpan expiration);
}

/// <summary>
/// Null object pattern implementation of caching interceptor that performs no operations.
/// </summary>
public sealed class NullCachingInterceptor : IOrderedProxyInterceptor
{
    /// <inheritdoc />
    public int Order => 150;

    /// <inheritdoc />
    public Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next, CancellationToken cancellationToken = default)
    {
        // Pass through without any caching
        return next(context, cancellationToken);
    }
}