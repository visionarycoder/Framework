using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Interface for security enrichers that add security-related data to proxy contexts.
/// </summary>
public interface ISecurityEnricher
{
    /// <summary>
    /// Enriches the proxy context with security-related information.
    /// </summary>
    /// <param name="context">The proxy context to enrich.</param>
    /// <returns>A task representing the asynchronous enrichment operation.</returns>
    Task EnrichAsync(ProxyContext context);

    /// <summary>
    /// Gets the order of execution for this enricher.
    /// </summary>
    int Order { get; }
}

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

/// <summary>
/// Security enricher that adds user information to the proxy context.
/// </summary>
public class UserContextEnricher : ISecurityEnricher
{
    private readonly IUserContextProvider _userProvider;

    /// <summary>
    /// Gets the execution order for this enricher.
    /// </summary>
    public int Order => 100;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserContextEnricher"/> class.
    /// </summary>
    /// <param name="userProvider">The user context provider.</param>
    public UserContextEnricher(IUserContextProvider userProvider)
    {
        _userProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));
    }

    /// <summary>
    /// Enriches the context with current user information.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>A task representing the enrichment operation.</returns>
    public async Task EnrichAsync(ProxyContext context)
    {
        var userContext = await _userProvider.GetCurrentUserAsync();
        if (userContext != null)
        {
            context.Metadata["UserId"] = userContext.UserId;
            context.Metadata["UserName"] = userContext.UserName;
            context.Metadata["Roles"] = userContext.Roles;
            context.Metadata["Permissions"] = userContext.Permissions;
        }
    }
}

/// <summary>
/// Security enricher that adds tenant information to the proxy context.
/// </summary>
public class TenantContextEnricher : ISecurityEnricher
{
    private readonly ITenantContextProvider _tenantProvider;

    /// <summary>
    /// Gets the execution order for this enricher.
    /// </summary>
    public int Order => 200;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantContextEnricher"/> class.
    /// </summary>
    /// <param name="tenantProvider">The tenant context provider.</param>
    public TenantContextEnricher(ITenantContextProvider tenantProvider)
    {
        _tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
    }

    /// <summary>
    /// Enriches the context with current tenant information.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>A task representing the enrichment operation.</returns>
    public async Task EnrichAsync(ProxyContext context)
    {
        var tenantContext = await _tenantProvider.GetCurrentTenantAsync();
        if (tenantContext != null)
        {
            context.Metadata["TenantId"] = tenantContext.TenantId;
            context.Metadata["TenantName"] = tenantContext.TenantName;
            context.Headers["X-Tenant-ID"] = tenantContext.TenantId;
        }
    }
}

/// <summary>
/// Role-based authorization policy.
/// </summary>
public class RoleBasedAuthorizationPolicy : IAuthorizationPolicy
{
    private readonly ICollection<string> _requiredRoles;

    /// <summary>
    /// Gets the name of the authorization policy.
    /// </summary>
    public string Name => "RoleBased";

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleBasedAuthorizationPolicy"/> class.
    /// </summary>
    /// <param name="requiredRoles">The roles required for authorization.</param>
    public RoleBasedAuthorizationPolicy(ICollection<string> requiredRoles)
    {
        _requiredRoles = requiredRoles ?? throw new ArgumentNullException(nameof(requiredRoles));
    }

    /// <summary>
    /// Evaluates role-based authorization.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>The authorization result.</returns>
    public Task<AuthorizationResult> EvaluateAsync(ProxyContext context)
    {
        if (!context.Metadata.TryGetValue("Roles", out var rolesObj) || 
            rolesObj is not ICollection<string> userRoles)
        {
            return Task.FromResult(AuthorizationResult.Failure("No roles found in context"));
        }

        var hasRequiredRole = _requiredRoles.Any(requiredRole => 
            userRoles.Contains(requiredRole, StringComparer.OrdinalIgnoreCase));

        return Task.FromResult(hasRequiredRole 
            ? AuthorizationResult.Success() 
            : AuthorizationResult.Failure($"User lacks required roles: {string.Join(", ", _requiredRoles)}"));
    }
}

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

/// <summary>
/// Represents tenant context information.
/// </summary>
public class TenantContext
{
    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tenant name.
    /// </summary>
    public string TenantName { get; set; } = string.Empty;
}

/// <summary>
/// Interface for providing user context information.
/// </summary>
public interface IUserContextProvider
{
    /// <summary>
    /// Gets the current user context.
    /// </summary>
    /// <returns>The current user context, or null if no user is authenticated.</returns>
    Task<UserContext?> GetCurrentUserAsync();
}

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