namespace VisionaryCoder.Framework.Proxy.Interceptors.Correlation.Abstractions;

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