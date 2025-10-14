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

    /// <summary>
    /// Gets or sets the reason for authorization failure.
    /// </summary>
    public string? FailureReason { get; set; }

    /// <summary>
    /// Gets or sets additional context about the authorization decision.
    /// </summary>
    public Dictionary<string, object> Context { get; set; } = new();

    /// <summary>
    /// Creates a successful authorization result.
    /// </summary>
    /// <returns>An authorized result.</returns>
    public static AuthorizationResult Success() => new() { IsAuthorized = true };

    /// <summary>
    /// Creates a failed authorization result.
    /// </summary>
    /// <param name="reason">The reason for failure.</param>
    /// <returns>An unauthorized result.</returns>
    public static AuthorizationResult Failure(string reason) => new() 
    { 
        IsAuthorized = false, 
        FailureReason = reason 
    };
}