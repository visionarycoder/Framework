using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VisionaryCoder.Framework.Secrets.Abstractions;

namespace VisionaryCoder.Framework.Azure.KeyVault;

/// <summary>
/// Extension methods for configuring Azure Key Vault secret services.
/// </summary>
public static class KeyVaultServiceCollectionExtensions
{
    /// <summary>
    /// Adds Azure Key Vault secret provider to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration to read settings from.</param>
    /// <param name="configure">Optional configuration action for KeyVaultOptions.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAzureKeyVaultSecrets(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<KeyVaultOptions>? configure = null)
    {
        services.AddMemoryCache();

        var options = new KeyVaultOptions();
        configuration.GetSection("KeyVault").Bind(options);
        configure?.Invoke(options);

        services.Configure<KeyVaultOptions>(opts =>
        {
            opts.VaultUri = options.VaultUri;
            opts.CacheTtl = options.CacheTtl;
            opts.UseLocalSecrets = options.UseLocalSecrets;
            opts.LocalSecretsPrefix = options.LocalSecretsPrefix;
            opts.MaxRetries = options.MaxRetries;
            opts.RetryDelay = options.RetryDelay;
        });

        // Local-first toggle (explicit) OR missing vault URI => local
        var useLocal = options.UseLocalSecrets || options.VaultUri is null;

        if (useLocal)
        {
            services.AddSingleton<ISecretProvider>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var opts = provider.GetRequiredService<IOptions<KeyVaultOptions>>();
                return new LocalSecretProvider(config, opts.Value);
            });
            return services;
        }

        // Configure Azure Key Vault client with managed identity
        services.AddSingleton(provider =>
        {
            var opts = provider.GetRequiredService<IOptions<KeyVaultOptions>>();
            
            // Use DefaultAzureCredential for managed identity support
            var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ExcludeInteractiveBrowserCredential = true // Better for production scenarios
            });

            return new SecretClient(opts.Value.VaultUri!, credential, new SecretClientOptions
            {
                Retry = { 
                    MaxRetries = opts.Value.MaxRetries, 
                    Delay = opts.Value.RetryDelay,
                    Mode = global::Azure.Core.RetryMode.Exponential 
                }
            });
        });

        services.AddSingleton<ISecretProvider, KeyVaultSecretProvider>();

        return services;
    }

    /// <summary>
    /// Adds a null secret provider (useful for testing or when secrets are not needed).
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddNullSecrets(this IServiceCollection services)
    {
        services.AddSingleton<ISecretProvider>(NullSecretProvider.Instance);
        return services;
    }
}