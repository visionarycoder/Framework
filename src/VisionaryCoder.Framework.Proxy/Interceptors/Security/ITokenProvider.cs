namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Interface for token providers in web scenarios.
/// </summary>
public interface ITokenProvider
{
    /// <summary>
    /// Gets a token asynchronously.
    /// </summary>
    /// <param name="request">The token request.</param>
    /// <returns>A task representing the token result.</returns>
    Task<TokenResult> GetTokenAsync(TokenRequest request);
}