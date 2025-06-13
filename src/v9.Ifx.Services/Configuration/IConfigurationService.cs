using System.Collections.Generic;
using System.Threading.Tasks;

namespace vc.Ifx.Services.Configuration;

/// <summary>
/// Provides functionality for reading and writing application configuration settings.
/// </summary>
public interface IConfigurationService : IService
{

    /// <summary>
    /// Gets the configuration file path.
    /// </summary>
    string ConfigurationFilePath { get; }

    /// <summary>
    /// Reads a configuration section as a strongly-typed object.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the configuration section to.</typeparam>
    /// <param name="sectionName">The name of the section to read. If null, reads the entire configuration.</param>
    /// <returns>The configuration object of type T, or default(T) if the section doesn't exist.</returns>
    T GetSection<T>(string sectionName = null) where T : class, new();

    /// <summary>
    /// Asynchronously reads a configuration section as a strongly-typed object.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the configuration section to.</typeparam>
    /// <param name="sectionName">The name of the section to read. If null, reads the entire configuration.</param>
    /// <returns>A task that represents the asynchronous operation. The value of the TResult parameter contains the configuration object of type T, or default(T) if the section doesn't exist.</returns>
    Task<T> GetSectionAsync<T>(string sectionName = null) where T : class, new();

    /// <summary>
    /// Gets a configuration value by key.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <param name="defaultValue">The default value to return if the key doesn't exist.</param>
    /// <returns>The configuration value converted to type T, or the default value if the key doesn't exist.</returns>
    T GetValue<T>(string key, T defaultValue = default);

    /// <summary>
    /// Gets all configuration values as a dictionary.
    /// </summary>
    /// <returns>A dictionary containing all configuration key-value pairs.</returns>
    IDictionary<string, object> GetAllValues();

    /// <summary>
    /// Updates a configuration section with the provided object.
    /// </summary>
    /// <typeparam name="T">The type of the configuration object.</typeparam>
    /// <param name="sectionName">The name of the section to update. If null, updates the entire configuration.</param>
    /// <param name="value">The configuration object to save.</param>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    bool UpdateSection<T>(string sectionName, T value) where T : class;

    /// <summary>
    /// Asynchronously updates a configuration section with the provided object.
    /// </summary>
    /// <typeparam name="T">The type of the configuration object.</typeparam>
    /// <param name="sectionName">The name of the section to update. If null, updates the entire configuration.</param>
    /// <param name="value">The configuration object to save.</param>
    /// <returns>A task that represents the asynchronous operation. The value of the TResult parameter contains true if the update was successful; otherwise, false.</returns>
    Task<bool> UpdateSectionAsync<T>(string sectionName, T value) where T : class;

    /// <summary>
    /// Sets a configuration value by key.
    /// </summary>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    bool SetValue<T>(string key, T value);

    /// <summary>
    /// Removes a configuration section.
    /// </summary>
    /// <param name="sectionName">The name of the section to remove.</param>
    /// <returns>True if the section was removed successfully; otherwise, false.</returns>
    bool RemoveSection(string sectionName);

    /// <summary>
    /// Removes a configuration value by key.
    /// </summary>
    /// <param name="key">The configuration key to remove.</param>
    /// <returns>True if the key was removed successfully; otherwise, false.</returns>
    bool RemoveValue(string key);

    /// <summary>
    /// Saves any pending changes to the configuration file.
    /// </summary>
    /// <returns>True if the save was successful; otherwise, false.</returns>
    bool SaveChanges();

    /// <summary>
    /// Asynchronously saves any pending changes to the configuration file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The value of the TResult parameter contains true if the save was successful; otherwise, false.</returns>
    Task<bool> SaveChangesAsync();

    /// <summary>
    /// Reloads the configuration from the file.
    /// </summary>
    /// <returns>True if the reload was successful; otherwise, false.</returns>
    bool Reload();

    /// <summary>
    /// Asynchronously reloads the configuration from the file.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The value of the TResult parameter contains true if the reload was successful; otherwise, false.</returns>
    Task<bool> ReloadAsync();

}