// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Represents the context of a proxy operation.
/// </summary>
/// <param name="request">The request object.</param>
/// <param name="resultType">The expected result type.</param>
/// <param name="cancellationToken">The cancellation token.</param>
public class ProxyContext(object request, Type resultType, CancellationToken cancellationToken = default)
{
    /// <summary>
    /// Gets the request object.
    /// </summary>
    public object Request { get; } = request ?? throw new ArgumentNullException(nameof(request));

    /// <summary>
    /// Gets the expected result type.
    /// </summary>
    public Type ResultType { get; } = resultType ?? throw new ArgumentNullException(nameof(resultType));

    /// <summary>
    /// Gets the cancellation token for the operation.
    /// </summary>
    public CancellationToken CancellationToken { get; } = cancellationToken;

    /// <summary>
    /// Gets or sets the correlation ID for the operation.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the operation name.
    /// </summary>
    public string? OperationName { get; set; }

    /// <summary>
    /// Gets or sets the unique request identifier.
    /// </summary>
    public string RequestId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the HTTP method for the request.
    /// </summary>
    public string Method { get; set; } = "GET";

    /// <summary>
    /// Gets or sets the request URL.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the request headers.
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Gets or sets additional items for the operation.
    /// </summary>
    public Dictionary<string, object> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets additional metadata for the operation.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Gets or sets the start time of the operation.
    /// </summary>
    public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;
}