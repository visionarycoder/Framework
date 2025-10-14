namespace VisionaryCoder.Framework.Azure.AppConfiguration;

/// <summary>
/// Configuration options for Azure App Configuration service integration.
/// </summary>
public sealed record AppConfigurationOptions
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
}