namespace VisionaryCoder.Framework.AppConfiguration;

/// <summary>
/// Defines the contract for application configuration providers.
/// Supports async operations, caching, refresh capabilities, and type-safe configuration access.
/// </summary>
public interface IAppConfigurationProvider
{
    /// <summary>
    /// Retrieves a strongly-typed configuration value by key.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the configuration value to</typeparam>
    /// <param name="key">The configuration key</param>
    /// <param name="defaultValue">The default value to return if the key is not found</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The configuration value or default value if not found</returns>
    Task<T> GetValueAsync<T>(string key, T defaultValue = default!, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a strongly-typed configuration section by name.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the configuration section to</typeparam>
    /// <param name="sectionName">The name of the configuration section</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deserialized configuration section</returns>
    Task<T> GetSectionAsync<T>(string sectionName, CancellationToken cancellationToken = default) where T : class, new();

    /// <summary>
    /// Retrieves all configuration values as a dictionary.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary containing all configuration key-value pairs</returns>
    Task<IDictionary<string, object?>> GetAllValuesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Forces a refresh of the configuration data from the underlying source.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if refresh was successful, false otherwise</returns>
    Task<bool> RefreshAsync(CancellationToken cancellationToken = default);
}