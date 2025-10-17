namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Represents a response from a proxy operation.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public class Response<T>
{
    /// <summary>
    /// Gets or sets the response data.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the status code.
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Creates a successful response.
    /// </summary>
    /// <param name="data">The response data.</param>
    /// <returns>A successful response.</returns>
    public static Response<T> Success(T data)
    {
        return new Response<T> { Data = data, IsSuccess = true };
    }

    /// <summary>
    /// Creates a successful response with status code.
    /// </summary>
    /// <param name="data">The response data.</param>
    /// <param name="statusCode">The status code.</param>
    /// <returns>A successful response.</returns>
    public static Response<T> Success(T data, int statusCode)
    {
        return new Response<T> { Data = data, IsSuccess = true, StatusCode = statusCode };
    }

    /// <summary>
    /// Creates a failed response.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed response.</returns>
    public static Response<T> Failure(string errorMessage)
    {
        return new Response<T> { IsSuccess = false, ErrorMessage = errorMessage };
    }
}

/// <summary>
/// Audit record for proxy operations.
/// </summary>
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
    /// Gets or sets the status code.
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

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

/// <summary>
/// Interface for audit sinks.
/// </summary>
public interface IAuditSink
{
    /// <summary>
    /// Writes an audit record asynchronously.
    /// </summary>
    /// <param name="record">The audit record to write.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteAsync(AuditRecord record);

    /// <summary>
    /// Emits an audit record asynchronously.
    /// </summary>
    /// <param name="record">The audit record to emit.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task EmitAsync(AuditRecord record) => WriteAsync(record);
}