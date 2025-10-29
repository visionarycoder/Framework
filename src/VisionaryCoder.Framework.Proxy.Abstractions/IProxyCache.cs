// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace VisionaryCoder.Framework.Proxy.Abstractions;
/// <summary>
/// Defines a contract for proxy caching operations.
/// </summary>
public interface IProxyCache
{
    /// <summary>
    /// Gets a cached value by key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>The cached value if found; otherwise, the default value.</returns>
    Task<T?> GetAsync<T>(string key);

    /// Sets a cached value.
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key"></param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">The expiration time.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    /// Removes a cached value by key.
    Task RemoveAsync(string key);
}
