using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;

namespace VisionaryCoder.Framework.Extensions.Configuration;

public static class ConfigurationServiceCollectionExtensions
{
    public static IServiceCollection AddSecretProvider(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<SecretOptions>? configure = null)
    {
        services.AddMemoryCache();

        var options = configuration.GetSection("Secrets").Get<SecretOptions>() ?? new SecretOptions();
        configure?.Invoke(options);

        // Local-first toggle (explicit) OR missing vault URI => local
        var useLocal = options.UseLocalSecrets || options.KeyVaultUri is null;

        if (useLocal)
        {
            services.AddSingleton<ISecretProvider, LocalSecretProvider>();
            return services;
        }

        // Best practice: DefaultAzureCredential (supports Managed Identity, dev tools, SPN)
        var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            // Respect AZURE_CLIENT_ID for user-assigned MI automatically
            ExcludeInteractiveBrowserCredential = true
        });

        var client = new SecretClient(options.KeyVaultUri, credential, new SecretClientOptions
        {
            Retry = { MaxRetries = 5, Mode = Azure.Core.RetryMode.Exponential }
        });

        services.AddSingleton(new SecretOptions
        {
            KeyVaultUri = options.KeyVaultUri,
            CacheTtl = options.CacheTtl,
            UseLocalSecrets = false
        });

        services.AddSingleton(client);
        services.AddSingleton<ISecretProvider, KeyVaultSecretProvider>();
        return services;
    }


}
