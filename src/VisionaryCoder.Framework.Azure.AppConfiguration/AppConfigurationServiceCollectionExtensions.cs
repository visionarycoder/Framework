using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;

namespace VisionaryCoder.Framework.Azure.AppConfiguration;

/// <summary>
/// Extension methods for configuring Azure App Configuration services.
/// </summary>
public static class AppConfigurationServiceCollectionExtensions
{
    /// <summary>
    /// Adds Azure App Configuration to the service collection with proper authentication and caching.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration to read settings from.</param>
    /// <param name="configure">Optional configuration action for AppConfigurationOptions.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAzureAppConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AppConfigurationOptions>? configure = null)
    {
        var options = configuration.GetSection("AzureAppConfiguration").Get<AppConfigurationOptions>() ?? new AppConfigurationOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        
        return services;
    }

    /// <summary>
    /// Adds Azure App Configuration to the configuration builder with proper authentication and refresh settings.
    /// </summary>
    /// <param name="builder">The configuration builder to configure.</param>
    /// <param name="options">The App Configuration options.</param>
    /// <returns>The configuration builder for chaining.</returns>
    public static IConfigurationBuilder AddAzureAppConfiguration(
        this IConfigurationBuilder builder,
        AppConfigurationOptions options)
    {
        if (options.Endpoint is null)
        {
            // Skip if no endpoint configured
            return builder;
        }

        return builder.AddAzureAppConfiguration(configOptions =>
        {
            // Use managed identity by default, connection string if specified
            if (options.UseConnectionString && !string.IsNullOrEmpty(options.ConnectionString))
            {
                configOptions.Connect(options.ConnectionString);
            }
            else
            {
                // Use DefaultAzureCredential for managed identity support
                var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ExcludeInteractiveBrowserCredential = true // Better for production scenarios
                });
                
                configOptions.Connect(options.Endpoint, credential);
            }

            // Select keys with the specified label
            configOptions.Select("*", options.Label)
                         .ConfigureRefresh(refresh =>
                         {
                             // Use sentinel key for refresh
                             refresh.Register(options.SentinelKey, options.Label)
                                   .SetRefreshInterval(options.CacheExpiration);
                         });
        });
    }
}