namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Represents the result of an authorization evaluation.
/// </summary>
public class AuthorizationResult
{
    /// <summary>
    /// Gets or sets a value indicating whether authorization succeeded.
    /// </summary>
    public bool IsAuthorized { get; set; }
    /// Gets or sets the reason for authorization failure.
    public string? FailureReason { get; set; }
    /// Gets or sets additional context about the authorization decision.
    public Dictionary<string, object> Context { get; set; } = new();
    /// Creates a successful authorization result.
    /// <returns>An authorized result.</returns>
    public static AuthorizationResult Success()
    {
        return new() { IsAuthorized = true };
    }
    /// Creates a failed authorization result.
    /// <param name="reason">The reason for failure.</param>
    /// <returns>An unauthorized result.</returns>
    public static AuthorizationResult Failure(string reason)
    {
        return new()
        {
            IsAuthorized = false,
            FailureReason = reason
        };
    }
}
