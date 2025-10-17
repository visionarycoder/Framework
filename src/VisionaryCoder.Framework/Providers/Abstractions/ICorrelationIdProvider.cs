namespace VisionaryCoder.Framework;

/// <summary>
/// Provides correlation ID generation and management.
/// </summary>
public interface ICorrelationIdProvider
{
    /// <summary>
    /// Gets the current correlation ID.
    /// </summary>
    string CorrelationId { get; }

    /// <summary>
    /// Generates a new correlation ID.
    /// </summary>
    /// <returns>A new correlation ID.</returns>
    string GenerateNew();

    /// <summary>
    /// Sets the current correlation ID.
    /// </summary>
    /// <param name="correlationId">The correlation ID to set.</param>
    void SetCorrelationId(string correlationId);
}