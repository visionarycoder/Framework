namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Interface for providing tenant context information.
/// </summary>
public interface ITenantContextProvider
{
    /// <summary>
    /// Gets the current tenant context.
    /// </summary>
    /// <returns>The current tenant context, or null if no tenant is set.</returns>
    Task<TenantContext?> GetCurrentTenantAsync();
}