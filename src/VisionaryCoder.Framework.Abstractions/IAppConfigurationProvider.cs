namespace VisionaryCoder.Framework.Abstractions;

/// <summary>
/// Defines a contract for application configuration providers that can retrieve and manage configuration values
/// from various sources such as Azure App Configuration, local files, or other configuration stores.
/// This interface supports both synchronous and asynchronous operations and follows the accessor pattern
/// for VBD (Volatility-Based Decomposition) architecture.
/// </summary>
/// <remarks>
/// This interface is designed to be easily mockable for unit testing and provides
/// consistent configuration access patterns across different backing stores.
/// Implementations should handle connection failures gracefully and provide fallback mechanisms.
/// </remarks>
public interface IAppConfigurationProvider
{
    // ==========================================
    // Configuration Value Operations
    // ==========================================

    /// <summary>
    /// Gets a configuration value by key.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <param name="defaultValue">The default value to return if the key doesn't exist.</param>
    /// <returns>The configuration value converted to type T, or the default value if the key doesn't exist.</returns>
    /// <exception cref="ArgumentException">Thrown when key is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the configuration provider is not available.</exception>
    T GetValue<T>(string key, T defaultValue = default!);

    /// <summary>
    /// Gets a configuration value by key asynchronously.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <param name="defaultValue">The default value to return if the key doesn't exist.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with the configuration value.</returns>
    /// <exception cref="ArgumentException">Thrown when key is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the configuration provider is not available.</exception>
    Task<T> GetValueAsync<T>(string key, T defaultValue = default!, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a configuration section as a strongly-typed object.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the configuration section to.</typeparam>
    /// <param name="sectionName">The name of the section to read.</param>
    /// <returns>The configuration object of type T, or a new instance of T if the section doesn't exist.</returns>
    /// <exception cref="ArgumentException">Thrown when sectionName is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the configuration provider is not available.</exception>
    T GetSection<T>(string sectionName) where T : class, new();

    /// <summary>
    /// Gets a configuration section as a strongly-typed object asynchronously.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the configuration section to.</typeparam>
    /// <param name="sectionName">The name of the section to read.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with the configuration object.</returns>
    /// <exception cref="ArgumentException">Thrown when sectionName is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the configuration provider is not available.</exception>
    Task<T> GetSectionAsync<T>(string sectionName, CancellationToken cancellationToken = default) where T : class, new();

    /// <summary>
    /// Gets all configuration values as a dictionary.
    /// </summary>
    /// <returns>A dictionary containing all configuration key-value pairs.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration provider is not available.</exception>
    IDictionary<string, object?> GetAllValues();

    /// <summary>
    /// Gets all configuration values as a dictionary asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with all configuration values.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration provider is not available.</exception>
    Task<IDictionary<string, object?>> GetAllValuesAsync(CancellationToken cancellationToken = default);

    // ==========================================
    // Configuration Management Operations
    // ==========================================

    /// <summary>
    /// Sets a configuration value by key (if supported by the provider).
    /// </summary>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when key is null or whitespace.</exception>
    /// <exception cref="NotSupportedException">Thrown when the provider doesn't support writing.</exception>
    bool SetValue<T>(string key, T value);

    /// <summary>
    /// Sets a configuration value by key asynchronously (if supported by the provider).
    /// </summary>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with success status.</returns>
    /// <exception cref="ArgumentException">Thrown when key is null or whitespace.</exception>
    /// <exception cref="NotSupportedException">Thrown when the provider doesn't support writing.</exception>
    Task<bool> SetValueAsync<T>(string key, T value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a configuration section with the provided object (if supported by the provider).
    /// </summary>
    /// <typeparam name="T">The type of the configuration object.</typeparam>
    /// <param name="sectionName">The name of the section to update.</param>
    /// <param name="value">The configuration object to save.</param>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when sectionName is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    /// <exception cref="NotSupportedException">Thrown when the provider doesn't support writing.</exception>
    bool UpdateSection<T>(string sectionName, T value) where T : class;

    /// <summary>
    /// Updates a configuration section with the provided object asynchronously (if supported by the provider).
    /// </summary>
    /// <typeparam name="T">The type of the configuration object.</typeparam>
    /// <param name="sectionName">The name of the section to update.</param>
    /// <param name="value">The configuration object to save.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with success status.</returns>
    /// <exception cref="ArgumentException">Thrown when sectionName is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    /// <exception cref="NotSupportedException">Thrown when the provider doesn't support writing.</exception>
    Task<bool> UpdateSectionAsync<T>(string sectionName, T value, CancellationToken cancellationToken = default) where T : class;

    // ==========================================
    // Provider Information and Health
    // ==========================================

    /// <summary>
    /// Gets a value indicating whether the configuration provider is currently available and ready to serve requests.
    /// </summary>
    /// <returns>True if the provider is available; otherwise, false.</returns>
    bool IsAvailable { get; }

    /// <summary>
    /// Gets the name or identifier of the configuration provider (e.g., "Azure", "Local", "Redis").
    /// </summary>
    /// <returns>The provider name.</returns>
    string ProviderName { get; }

    /// <summary>
    /// Refreshes the configuration from the underlying source (if supported by the provider).
    /// </summary>
    /// <returns>True if the refresh was successful; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration provider is not available.</exception>
    bool Refresh();

    /// <summary>
    /// Refreshes the configuration from the underlying source asynchronously (if supported by the provider).
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with success status.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration provider is not available.</exception>
    Task<bool> RefreshAsync(CancellationToken cancellationToken = default);
}