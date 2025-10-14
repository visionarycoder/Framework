// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace VisionaryCoder.Framework.Proxy.Interceptors.Correlation;

/// <summary>
/// Defines a contract for correlation context management.
/// </summary>
public interface ICorrelationContext
{
    /// <summary>
    /// Gets the current correlation ID.
    /// </summary>
    string? CorrelationId { get; }

    /// <summary>
    /// Sets the correlation ID for the current context.
    /// </summary>
    /// <param name="correlationId">The correlation ID to set.</param>
    void SetCorrelationId(string correlationId);
}

/// <summary>
/// Defines a contract for generating correlation IDs.
/// </summary>
public interface ICorrelationIdGenerator
{
    /// <summary>
    /// Generates a new correlation ID.
    /// </summary>
    /// <returns>A new correlation ID.</returns>
    string GenerateId();
}