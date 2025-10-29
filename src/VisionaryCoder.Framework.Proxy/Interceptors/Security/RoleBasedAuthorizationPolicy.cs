using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;
/// <summary>
/// Role-based authorization policy.
/// </summary>
/// <param name="requiredRoles">The roles required for authorization.</param>
public class RoleBasedAuthorizationPolicy(ICollection<string> requiredRoles) : IAuthorizationPolicy
{
    private readonly ICollection<string> requiredRoles = requiredRoles ?? throw new ArgumentNullException(nameof(requiredRoles));
    /// <summary>
    /// Gets the name of the authorization policy.
    /// </summary>
    public string Name => "RoleBased";
    /// Evaluates role-based authorization.
    /// <param name="context">The proxy context.</param>
    /// <returns>The authorization result.</returns>
    public Task<AuthorizationResult> EvaluateAsync(ProxyContext context)
    {
        if (!context.Metadata.TryGetValue("Roles", out object? rolesObj) || 
            rolesObj is not ICollection<string> userRoles)
        {
            return Task.FromResult(AuthorizationResult.Failure("No roles found in context"));
        }
        bool hasRequiredRole = requiredRoles.Any(requiredRole => 
            userRoles.Contains(requiredRole, StringComparer.OrdinalIgnoreCase));
        return Task.FromResult(hasRequiredRole 
            ? AuthorizationResult.Success() 
            : AuthorizationResult.Failure($"User lacks required roles: {string.Join(", ", requiredRoles)}"));
    }
}
