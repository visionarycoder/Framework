namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Defines a contract for authorization policies.
/// </summary>
public interface IAuthorizationPolicy
{
    /// <summary>
    /// Determines whether the operation is authorized.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>A task representing the asynchronous operation with the authorization result.</returns>
    Task<bool> IsAuthorizedAsync(ProxyContext context);
}
