namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Represents a token request for web authentication.
/// </summary>
public class TokenRequest
{
    /// <summary>
    /// Gets or sets the audience for the token.
    /// </summary>
    public string Audience { get; set; } = string.Empty;
    /// Gets or sets the scopes to request.
    public ICollection<string> Scopes { get; set; } = new List<string>();
    /// Gets or sets a value indicating whether to refresh if expired.
    public bool RefreshIfExpired { get; set; } = true;
}
