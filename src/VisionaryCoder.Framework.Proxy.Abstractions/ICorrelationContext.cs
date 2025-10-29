// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace VisionaryCoder.Framework.Proxy.Abstractions;
/// <summary>
/// Defines a contract for correlation context management.
/// </summary>
public interface ICorrelationContext
{
    /// <summary>
    /// Gets the current correlation ID.
    /// </summary>
    string? CorrelationId { get; }
    /// Sets the correlation ID for the current context.
    /// <param name="correlationId">The correlation ID to set.</param>
    void SetCorrelationId(string correlationId);
}
