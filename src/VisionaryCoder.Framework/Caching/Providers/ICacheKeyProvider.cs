// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using VisionaryCoder.Framework.Proxy;

namespace VisionaryCoder.Framework.Caching.Providers;

/// <summary>
/// Defines a contract for generating cache keys based on proxy context.
/// Implementations should create consistent, unique keys that properly identify cached content.
/// </summary>
public interface ICacheKeyProvider
{
    /// <summary>
    /// Generates a cache key for the given proxy context.
    /// The key should be unique and deterministic for the same context inputs.
    /// </summary>
    /// <param name="context">The proxy context containing request information.</param>
    /// <returns>A unique cache key string for the context.</returns>
    string GenerateKey(ProxyContext context);
}