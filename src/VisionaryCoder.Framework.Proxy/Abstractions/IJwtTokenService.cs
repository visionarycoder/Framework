namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Defines a contract for JWT token services.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Validates a JWT token.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>A task representing the asynchronous operation with the validation result.</returns>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// Gets claims from a JWT token.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <returns>A dictionary of claims.</returns>
    Task<Dictionary<string, object>> GetClaimsAsync(string token);
}