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

    /// <summary>
    /// Sets the correlation ID for the current context.
    /// </summary>
    /// <param name="correlationId">The correlation ID to set.</param>
    void SetCorrelationId(string correlationId);
}

/// <summary>
/// Defines a contract for correlation ID generators.
/// </summary>
public interface ICorrelationIdGenerator
{
    /// <summary>
    /// Generates a new correlation ID.
    /// </summary>
    /// <returns>A new correlation ID.</returns>
    string GenerateCorrelationId();
}



/// <summary>
/// Defines a contract for proxy error classifiers.
/// </summary>
public interface IProxyErrorClassifier
{
    /// <summary>
    /// Determines whether an exception should be retried.
    /// </summary>
    /// <param name="exception">The exception to classify.</param>
    /// <returns>True if the exception should be retried; otherwise, false.</returns>
    bool ShouldRetry(Exception exception);

    /// <summary>
    /// Determines whether an exception is transient.
    /// </summary>
    /// <param name="exception">The exception to classify.</param>
    /// <returns>True if the exception is transient; otherwise, false.</returns>
    bool IsTransient(Exception exception);
}