namespace VisionaryCoder.Framework.AppConfiguration.Local;

/// <summary>
/// Configuration options for Local (file-based) App Configuration provider.
/// </summary>
public sealed class LocalAppConfigurationProviderOptions
{
    /// <summary>
    /// The file path for the configuration file.
    /// </summary>
    public string FilePath { get; init; } = "appsettings.json";

    /// <summary>
    /// Whether to watch the file for changes and automatically reload.
    /// </summary>
    public bool ReloadOnChange { get; init; } = true;

    /// <summary>
    /// Whether the configuration file is optional.
    /// </summary>
    public bool Optional { get; init; } = false;

    /// <summary>
    /// Additional configuration files to include (e.g., environment-specific files).
    /// </summary>
    public IEnumerable<string> AdditionalFiles { get; init; } = Array.Empty<string>();

    /// <summary>
    /// The base path for configuration files.
    /// </summary>
    public string? BasePath { get; init; }

    /// <summary>
    /// The prefix to filter configuration keys (optional).
    /// </summary>
    public string? KeyPrefix { get; init; }

    /// <summary>
    /// Whether to enable caching of configuration values.
    /// </summary>
    public bool EnableCaching { get; init; } = true;

    /// <summary>
    /// The cache expiration time for configuration values.
    /// </summary>
    public TimeSpan CacheExpiration { get; init; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Validates the configuration options.
    /// </summary>
    internal void Validate()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
            throw new InvalidOperationException("FilePath cannot be null or empty.");

        if (CacheExpiration <= TimeSpan.Zero)
            throw new InvalidOperationException("CacheExpiration must be greater than zero.");

        // Validate additional files
        foreach (string file in AdditionalFiles)
        {
            if (string.IsNullOrWhiteSpace(file))
                throw new InvalidOperationException("Additional file paths cannot be null or empty.");
        }
    }
}