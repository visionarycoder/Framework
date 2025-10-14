using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Interface for authorization policies that determine access permissions.
/// </summary>
public interface IAuthorizationPolicy
{
    /// <summary>
    /// Gets the name of the authorization policy.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Evaluates whether the proxy context satisfies the authorization policy.
    /// </summary>
    /// <param name="context">The proxy context to evaluate.</param>
    /// <returns>A task representing the authorization result.</returns>
    Task<AuthorizationResult> EvaluateAsync(ProxyContext context);
}