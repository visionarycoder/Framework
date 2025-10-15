namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Represents user context information.
/// </summary>
public class UserContext
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user roles.
    /// </summary>
    public ICollection<string> Roles { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the user permissions.
    /// </summary>
    public ICollection<string> Permissions { get; set; } = new List<string>();
}