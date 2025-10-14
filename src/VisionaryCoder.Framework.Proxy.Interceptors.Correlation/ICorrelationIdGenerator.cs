namespace VisionaryCoder.Framework.Proxy.Interceptors.Correlation;

/// <summary>
/// Defines a contract for generating correlation IDs.
/// </summary>
public interface ICorrelationIdGenerator
{
    /// <summary>
    /// Generates a new correlation ID.
    /// </summary>
    /// <returns>A new correlation ID.</returns>
    string GenerateId();
}