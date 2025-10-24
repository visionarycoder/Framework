using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;
/// <summary>
/// Security enricher that adds user information to the proxy context.
/// </summary>
/// <param name="userProvider">The user context provider.</param>
public class UserContextEnricher(IUserContextProvider userProvider) : ISecurityEnricher
{
    private readonly IUserContextProvider userProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));
    /// <summary>
    /// Gets the execution order for this enricher.
    /// </summary>
    public int Order => 100;
    /// Enriches the context with current user information.
    /// <param name="context">The proxy context.</param>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the enrichment operation.</returns>
    public async Task EnrichAsync(ProxyContext context, CancellationToken cancellationToken = default)
    {
        var userContext = await userProvider.GetCurrentUserAsync(cancellationToken);
        if (userContext != null)
        {
            context.Metadata["UserId"] = userContext.UserId;
            context.Metadata["UserName"] = userContext.UserName;
            context.Metadata["Roles"] = userContext.Roles;
            context.Metadata["Permissions"] = userContext.Permissions;
        }
    }
}
