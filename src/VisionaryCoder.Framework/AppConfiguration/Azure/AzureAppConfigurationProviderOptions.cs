namespace VisionaryCoder.Framework.AppConfiguration.Azure;

/// <summary>
/// Configuration options for Azure App Configuration provider.
/// </summary>
public sealed class AzureAppConfigurationProviderOptions
{
    /// <summary>
    /// The endpoint URI for the Azure App Configuration service.
    /// </summary>
    /// <example>https://your-config.azconfig.io</example>
    public Uri? Endpoint { get; init; }

    /// <summary>
    /// The label to use for environment-specific configuration (e.g., "Development", "Testing", "Staging", "Production").
    /// </summary>
    public string Label { get; init; } = "Production";

    /// <summary>
    /// The sentinel key used to trigger configuration refresh.
    /// </summary>
    public string SentinelKey { get; init; } = "App:Sentinel";

    /// <summary>
    /// The cache expiration time for configuration values.
    /// </summary>
    public TimeSpan CacheExpiration { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Whether to use connection string authentication instead of managed identity.
    /// </summary>
    public bool UseConnectionString { get; init; } = false;
    
    /// <summary>
    /// The connection string for Azure App Configuration (when UseConnectionString is true).
    /// </summary>
    public string? ConnectionString { get; init; }

    /// <summary>
    /// Whether to enable automatic refresh of configuration values.
    /// </summary>
    public bool EnableRefresh { get; init; } = true;

    /// <summary>
    /// The prefix to filter configuration keys (optional).
    /// </summary>
    public string? KeyPrefix { get; init; }

    /// <summary>
    /// Validates the configuration options.
    /// </summary>
    internal void Validate()
    {
        if (UseConnectionString)
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new InvalidOperationException("ConnectionString must be provided when UseConnectionString is true.");
        }
        else
        {
            if (Endpoint is null)
                throw new InvalidOperationException("Endpoint must be provided when not using connection string authentication.");
        }

        if (string.IsNullOrWhiteSpace(Label))
            throw new InvalidOperationException("Label cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(SentinelKey))
            throw new InvalidOperationException("SentinelKey cannot be null or empty.");

        if (CacheExpiration <= TimeSpan.Zero)
            throw new InvalidOperationException("CacheExpiration must be greater than zero.");
    }
}