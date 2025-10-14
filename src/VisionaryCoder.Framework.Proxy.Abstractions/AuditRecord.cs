// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Represents an audit record.
/// </summary>
public record AuditRecord
{
    /// <summary>
    /// Gets or sets the correlation ID.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Gets or sets the operation name.
    /// </summary>
    public string? OperationName { get; init; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the operation result.
    /// </summary>
    public string? Result { get; init; }

    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = new();
}