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
    /// Gets or sets the access token.
    public string AccessToken { get; set; } = string.Empty;
    /// Gets or sets the error message if the request failed.
    public string Error { get; set; } = string.Empty;
    /// Gets or sets the correlation ID for the request.
    public string CorrelationId { get; set; } = string.Empty;
    /// Gets or sets the token expiration time.
    public DateTimeOffset? ExpiresAt { get; set; }
}
