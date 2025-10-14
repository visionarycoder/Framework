using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security.Abstractions;

/// <summary>
/// Defines a contract for authorization policies.
/// </summary>
public interface IProxyAuthorizationPolicy
{
    /// <summary>
    /// Determines whether the current context is authorized for the operation.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a boolean result indicating authorization status.</returns>
    Task<bool> IsAuthorizedAsync(ProxyContext context, CancellationToken cancellationToken = default);
}