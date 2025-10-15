// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace VisionaryCoder.Framework.Proxy.Interceptors.Auditing.Abstractions;

/// <summary>
/// Represents an audit record for proxy operations.
/// </summary>
public sealed record AuditRecord(string CorrelationId, string Operation, string RequestType, DateTime Timestamp, bool Success, string? Error = null, TimeSpan? Duration = null, Dictionary<string, object?>? Metadata = null);