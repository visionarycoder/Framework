using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisionaryCoder.Framework.Secrets.Abstractions;

namespace VisionaryCoder.Framework.Data.Configuration;

/// <summary>
/// Extension methods for configuring database connections and connection strings.
/// </summary>
public static class DataConfigurationServiceCollectionExtensions
{
    /// <summary>
    /// Adds a connection string from configuration to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the connection string to.</param>
    /// <param name="configuration">The configuration containing the connection string.</param>
    /// <param name="connectionName">The name of the connection string in configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddConnectionString(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionName)
    {
        var connectionStringValue = configuration.GetConnectionString(connectionName);
        
        if (string.IsNullOrWhiteSpace(connectionStringValue))
        {
            throw new InvalidOperationException($"Connection string '{connectionName}' is not configured.");
        }

        var connectionString = ConnectionString.Create(connectionStringValue);
        services.AddSingleton(connectionString);
        
        return services;
    }

    /// <summary>
    /// Adds a named connection string from configuration to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the connection string to.</param>
    /// <param name="configuration">The configuration containing the connection string.</param>
    /// <param name="connectionName">The name of the connection string in configuration.</param>
    /// <param name="serviceName">The service name to register the connection string under.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddNamedConnectionString(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionName,
        string serviceName)
    {
        var connectionStringValue = configuration.GetConnectionString(connectionName);
        
        if (string.IsNullOrWhiteSpace(connectionStringValue))
        {
            throw new InvalidOperationException($"Connection string '{connectionName}' is not configured.");
        }

        var connectionString = ConnectionString.Create(connectionStringValue);
        services.AddKeyedSingleton(serviceName, connectionString);
        
        return services;
    }

    /// <summary>
    /// Adds a connection string from a secret provider to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the connection string to.</param>
    /// <param name="secretName">The name of the secret containing the connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddConnectionStringFromSecret(
        this IServiceCollection services,
        string secretName)
    {
        services.AddSingleton<ConnectionString>(provider =>
        {
            var secretProvider = provider.GetRequiredService<ISecretProvider>();
            var connectionStringValue = secretProvider.GetAsync(secretName).GetAwaiter().GetResult();
            
            if (string.IsNullOrWhiteSpace(connectionStringValue))
            {
                throw new InvalidOperationException($"Connection string secret '{secretName}' is not available or empty.");
            }

            return ConnectionString.Create(connectionStringValue);
        });

        return services;
    }
}