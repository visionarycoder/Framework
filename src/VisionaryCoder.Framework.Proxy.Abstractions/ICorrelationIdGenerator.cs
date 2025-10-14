namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Defines a contract for correlation ID generators.
/// </summary>
public interface ICorrelationIdGenerator
{
    /// <summary>
    /// Generates a new correlation ID.
    /// </summary>
    /// <returns>A new correlation ID.</returns>
    string GenerateCorrelationId();
}