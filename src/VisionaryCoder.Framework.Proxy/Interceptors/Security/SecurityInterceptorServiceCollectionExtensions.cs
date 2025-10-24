using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Abstractions.Services;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;
/// <summary>
/// Extension methods for adding security interceptor services.
/// </summary>
public static class SecurityInterceptorServiceCollectionExtensions
{
    /// <summary>
    /// Adds the JWT Bearer interceptor to the service collection with a token provider function.
    /// </summary>
    /// <param name="services">The service collection to add the interceptor to.</param>
    /// <param name="tokenProvider">Function that provides JWT tokens.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddJwtBearerInterceptor(
        this IServiceCollection services,
        Func<CancellationToken, Task<string?>> tokenProvider)
    {
        services.AddSingleton<IProxyInterceptor>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<JwtBearerInterceptor>>();
            return new JwtBearerInterceptor(logger, tokenProvider);
        });
        return services;
    }
    /// Adds the JWT Bearer interceptor that retrieves tokens from a secret provider.
    /// <param name="secretName">The name of the secret containing the JWT token.</param>
        string secretName)
            var secretProvider = provider.GetRequiredService<ISecretProvider>();
            Func<CancellationToken, Task<string?>> tokenProvider = async (cancellationToken) =>
            {
                return await secretProvider.GetAsync(secretName, cancellationToken);
            };
    /// Adds the JWT Bearer interceptor with a static token (useful for development).
    /// <param name="staticToken">The static JWT token to use.</param>
    public static IServiceCollection AddJwtBearerInterceptorWithStaticToken(
        string staticToken)
        return services.AddJwtBearerInterceptor((cancellationToken) => Task.FromResult<string?>(staticToken));
}
