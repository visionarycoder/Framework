namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Represents an audit record for proxy operations.
/// </summary>
public class AuditRecord
{
    /// <summary>
    /// Gets or sets the unique request identifier.
    /// </summary>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user ID who made the request.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the user agent string.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the IP address of the client.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method.
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the request URL.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets when the request started.
    /// </summary>
    public DateTimeOffset StartedAt { get; set; }

    /// <summary>
    /// Gets or sets when the request completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the request duration.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code.
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Gets or sets whether the request was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the error message if the request failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the exception type if an error occurred.
    /// </summary>
    public string? ExceptionType { get; set; }

    /// <summary>
    /// Gets or sets the request headers (sanitized).
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Gets or sets the request size in bytes.
    /// </summary>
    public long RequestSize { get; set; }

    /// <summary>
    /// Gets or sets the response size in bytes.
    /// </summary>
    public long? ResponseSize { get; set; }
}