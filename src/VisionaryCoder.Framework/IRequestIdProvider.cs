namespace VisionaryCoder.Framework;

/// <summary>
/// Provides request ID generation and management.
/// </summary>
public interface IRequestIdProvider
{
    /// <summary>
    /// Gets the current request ID.
    /// </summary>
    string RequestId { get; }

    /// <summary>
    /// Generates a new request ID.
    /// </summary>
    /// <returns>A new request ID.</returns>
    string GenerateNew();

    /// <summary>
    /// Sets the current request ID.
    /// </summary>
    /// <param name="requestId">The request ID to set.</param>
    void SetRequestId(string requestId);
}