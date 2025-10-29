namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// Audit record for proxy operations.
public class AuditRecord
{
    /// <summary>
    /// Gets or sets the operation identifier.
    /// </summary>
    public string OperationId { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the method name.
    /// </summary>
    public string MethodName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the operation name (alias for MethodName).
    /// </summary>
    public string OperationName { get => MethodName; set => MethodName = value; }
    /// <summary>
    /// Gets or sets the result of the operation.
    /// </summary>
    public object? Result { get; set; }
    /// <summary>
    /// Gets or sets the timestamp of the operation.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Gets or sets the duration of the operation.
    /// </summary>
    public TimeSpan Duration { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// Gets or sets any error message.
    /// </summary>
    public string? ErrorMessage { get; set; }
    /// <summary>
    /// Gets or sets the correlation identifier.
    /// </summary>
    public string? CorrelationId { get; set; }
    /// <summary>
    /// Gets or sets the request identifier.
    /// </summary>
    public string? RequestId { get; set; }
    /// <summary>
    /// Gets or sets when the operation completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    /// <summary>
    /// Gets or sets the exception type if an error occurred.
    /// </summary>
    public string? ExceptionType { get; set; }
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public string? UserId { get; set; }
    /// <summary>
    /// Gets or sets the user agent.
    /// </summary>
    public string? UserAgent { get; set; }
    /// <summary>
    /// Gets or sets the IP address.
    /// </summary>
    public string? IpAddress { get; set; }
    /// <summary>
    /// Gets or sets the HTTP method.
    /// </summary>
    public string? Method { get; set; }
    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string? Url { get; set; }
    /// <summary>
    /// Gets or sets when the operation started.
    /// </summary>
    public DateTime? StartedAt { get; set; }
    /// <summary>
    /// Gets or sets the request headers.
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }
    /// <summary>
    /// Gets or sets the request size.
    /// </summary>
    public long? RequestSize { get; set; }
    /// <summary>
    /// Gets or sets the response size.
    /// </summary>
    public long? ResponseSize { get; set; }
}