// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Defines a contract for ordered proxy interceptors.
/// </summary>
public interface IOrderedProxyInterceptor : IProxyInterceptor
{
    /// <summary>
    /// Gets the order in which this interceptor should be executed.
    /// Lower values execute first.
    /// </summary>
    int Order { get; }
}
