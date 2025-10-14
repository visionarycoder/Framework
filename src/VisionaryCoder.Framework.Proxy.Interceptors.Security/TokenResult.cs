namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Represents the result of a token request.
/// </summary>
public class TokenResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the request was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message if the request failed.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the correlation ID for the request.
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token expiration time.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }
}