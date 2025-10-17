namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Interface for providing tenant context information.
/// </summary>
public interface ITenantContextProvider
{
    /// <summary>
    /// Gets the current tenant context.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>The current tenant context, or null if no tenant is set.</returns>
    Task<TenantContext?> GetCurrentTenantAsync(CancellationToken cancellationToken = default);
}