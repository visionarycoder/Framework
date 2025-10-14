namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Configuration options for web JWT authentication.
/// </summary>
public class WebJwtOptions
{
    /// <summary>
    /// Gets or sets the audience for the JWT token.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the scopes to request with the token.
    /// </summary>
    public ICollection<string> Scopes { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the header name to add the token to.
    /// </summary>
    public string HeaderName { get; set; } = "Authorization";

    /// <summary>
    /// Gets or sets a value indicating whether to refresh the token if expired.
    /// </summary>
    public bool RefreshIfExpired { get; set; } = true;
}