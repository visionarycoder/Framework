using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;
/// <summary>
/// Security enricher that adds tenant information to the proxy context.
/// </summary>
/// <param name="tenantProvider">The tenant context provider.</param>
public class TenantContextEnricher(ITenantContextProvider tenantProvider) : ISecurityEnricher
{
    private readonly ITenantContextProvider tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
    /// <summary>
    /// Gets the execution order for this enricher.
    /// </summary>
    public int Order => 200;
    /// Enriches the context with current tenant information.
    /// <param name="context">The proxy context.</param>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the enrichment operation.</returns>
    public async Task EnrichAsync(ProxyContext context, CancellationToken cancellationToken = default)
    {
        TenantContext? tenantContext = await tenantProvider.GetCurrentTenantAsync(cancellationToken);
        if (tenantContext != null)
        {
            context.Metadata["TenantId"] = tenantContext.TenantId;
            context.Metadata["TenantName"] = tenantContext.TenantName;
            context.Headers["X-Tenant-ID"] = tenantContext.TenantId;
        }
    }
}
