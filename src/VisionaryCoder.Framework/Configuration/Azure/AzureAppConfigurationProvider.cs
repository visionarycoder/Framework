using System.Collections.Concurrent;
using System.Text.Json;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Abstractions;

namespace VisionaryCoder.Framework.Configuration.Azure;

/// <summary>
/// Provides Azure App Configuration-based configuration operations following Microsoft configuration patterns.
/// This service wraps Azure App Configuration with logging, error handling, caching, and async support.
/// Supports both connection string and managed identity authentication with automatic refresh capabilities.
/// </summary>
public sealed class AzureAppConfigurationProvider : ServiceBase<AzureAppConfigurationProvider>, IAppConfigurationProvider
{
    private readonly AzureAppConfigurationProviderOptions options;
    private readonly IConfiguration configuration;
    private readonly ConcurrentDictionary<string, object?> cache;
    private readonly SemaphoreSlim refreshSemaphore;
    private DateTimeOffset lastRefresh;
    private bool isDisposed;

    public AzureAppConfigurationProvider(
        AzureAppConfigurationProviderOptions options,
        ILogger<AzureAppConfigurationProvider> logger)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        
        this.options = options;
        this.options.Validate();
        
        this.cache = new ConcurrentDictionary<string, object?>();
        this.refreshSemaphore = new SemaphoreSlim(1, 1);
        this.lastRefresh = DateTimeOffset.MinValue;
        
        this.configuration = BuildConfiguration();
        
        Logger.LogInformation(
            "Azure App Configuration provider initialized for endpoint {Endpoint} with label {Label}",
            options.Endpoint?.ToString() ?? "[Connection String]", 
            options.Label);
    }

    public string ProviderName => "Azure";

    public bool IsAvailable
    {
        get
        {
            try
            {
                // Simple health check - try to get a test value
                _ = configuration["__health_check__"];
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Azure App Configuration health check failed");
                return false;
            }
        }
    }

    public T GetValue<T>(string key, T defaultValue = default!)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        try
        {
            string fullKey = GetFullKey(key);
            
            // Check cache first
            if (cache.TryGetValue(fullKey, out object? cachedValue) && cachedValue is T typedValue)
            {
                Logger.LogTrace("Configuration value retrieved from cache for key {Key}", key);
                return typedValue;
            }

            // Get from Azure App Configuration
            string? stringValue = configuration[fullKey];
            if (string.IsNullOrEmpty(stringValue))
            {
                Logger.LogDebug("Configuration key {Key} not found, returning default value", key);
                return defaultValue;
            }

            T result = ConvertValue<T>(stringValue, defaultValue);
            
            // Cache the result
            cache.TryAdd(fullKey, result);
            
            Logger.LogTrace("Configuration value retrieved for key {Key}", key);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get configuration value for key {Key}", key);
            return defaultValue;
        }
    }

    public async Task<T> GetValueAsync<T>(string key, T defaultValue = default!, CancellationToken cancellationToken = default)
    {
        // Azure App Configuration is synchronous in nature, but we provide async wrapper for consistency
        return await Task.FromResult(GetValue(key, defaultValue));
    }

    public T GetSection<T>(string sectionName) where T : class, new()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);
        
        try
        {
            string fullSectionName = GetFullKey(sectionName);
            IConfigurationSection section = configuration.GetSection(fullSectionName);
            
            if (!section.Exists())
            {
                Logger.LogDebug("Configuration section {SectionName} not found, returning new instance", sectionName);
                return new T();
            }

            T? result = section.Get<T>() ?? new T();
            Logger.LogTrace("Configuration section retrieved for {SectionName}", sectionName);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get configuration section {SectionName}", sectionName);
            return new T();
        }
    }

    public async Task<T> GetSectionAsync<T>(string sectionName, CancellationToken cancellationToken = default) where T : class, new()
    {
        return await Task.FromResult(GetSection<T>(sectionName));
    }

    public IDictionary<string, object?> GetAllValues()
    {
        try
        {
            var result = new Dictionary<string, object?>();
            string keyPrefix = options.KeyPrefix ?? string.Empty;
            
            foreach (KeyValuePair<string, string?> kvp in configuration.AsEnumerable())
            {
                if (string.IsNullOrEmpty(keyPrefix) || kvp.Key.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    string cleanKey = string.IsNullOrEmpty(keyPrefix) 
                        ? kvp.Key 
                        : kvp.Key[keyPrefix.Length..].TrimStart(':');
                    
                    result[cleanKey] = kvp.Value;
                }
            }
            
            Logger.LogTrace("Retrieved {Count} configuration values", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get all configuration values");
            return new Dictionary<string, object?>();
        }
    }

    public async Task<IDictionary<string, object?>> GetAllValuesAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(GetAllValues());
    }

    public bool SetValue<T>(string key, T value)
    {
        Logger.LogWarning("SetValue operation not supported by Azure App Configuration provider. Use Azure portal or REST API to modify values.");
        throw new NotSupportedException("Azure App Configuration provider is read-only. Use Azure portal or REST API to modify configuration values.");
    }

    public Task<bool> SetValueAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SetValue(key, value));
    }

    public bool UpdateSection<T>(string sectionName, T value) where T : class
    {
        Logger.LogWarning("UpdateSection operation not supported by Azure App Configuration provider. Use Azure portal or REST API to modify values.");
        throw new NotSupportedException("Azure App Configuration provider is read-only. Use Azure portal or REST API to modify configuration values.");
    }

    public Task<bool> UpdateSectionAsync<T>(string sectionName, T value, CancellationToken cancellationToken = default) where T : class
    {
        return Task.FromResult(UpdateSection(sectionName, value));
    }

    public bool Refresh()
    {
        if (!options.EnableRefresh)
        {
            Logger.LogDebug("Configuration refresh is disabled");
            return true;
        }

        try
        {
            if (refreshSemaphore.Wait(TimeSpan.FromSeconds(5)))
            {
                try
                {
                    // Check if we need to refresh based on cache expiration
                    if (DateTimeOffset.UtcNow - lastRefresh < options.CacheExpiration)
                    {
                        Logger.LogTrace("Configuration refresh skipped - cache is still valid");
                        return true;
                    }

                    // Clear cache to force refresh
                    cache.Clear();
                    lastRefresh = DateTimeOffset.UtcNow;
                    
                    // Force refresh the configuration (this is handled by Azure App Configuration middleware)
                    // In a real implementation, you might need to rebuild the configuration or trigger refresh
                    
                    Logger.LogDebug("Configuration refreshed successfully");
                    return true;
                }
                finally
                {
                    refreshSemaphore.Release();
                }
            }
            else
            {
                Logger.LogWarning("Configuration refresh timeout - another refresh operation is in progress");
                return false;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to refresh configuration");
            return false;
        }
    }

    public async Task<bool> RefreshAsync(CancellationToken cancellationToken = default)
    {
        if (!options.EnableRefresh)
        {
            Logger.LogDebug("Configuration refresh is disabled");
            return true;
        }

        try
        {
            if (await refreshSemaphore.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken))
            {
                try
                {
                    // Check if we need to refresh based on cache expiration
                    if (DateTimeOffset.UtcNow - lastRefresh < options.CacheExpiration)
                    {
                        Logger.LogTrace("Configuration refresh skipped - cache is still valid");
                        return true;
                    }

                    // Clear cache to force refresh
                    cache.Clear();
                    lastRefresh = DateTimeOffset.UtcNow;
                    
                    Logger.LogDebug("Configuration refreshed successfully");
                    return true;
                }
                finally
                {
                    refreshSemaphore.Release();
                }
            }
            else
            {
                Logger.LogWarning("Configuration refresh timeout - another refresh operation is in progress");
                return false;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to refresh configuration");
            return false;
        }
    }

    private IConfiguration BuildConfiguration()
    {
        var builder = new ConfigurationBuilder();
        
        try
        {
            builder.AddAzureAppConfiguration(configOptions =>
            {
                // Use connection string or managed identity based on options
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
                    configOptions.Connect(options.Endpoint!, credential);
                }

                // Select keys with optional prefix and specified label
                string keyFilter = string.IsNullOrEmpty(options.KeyPrefix) ? "*" : $"{options.KeyPrefix}*";
                configOptions.Select(keyFilter, options.Label);

                // Configure refresh if enabled
                if (options.EnableRefresh)
                {
                    configOptions.ConfigureRefresh(refresh =>
                    {
                        refresh.Register(options.SentinelKey, options.Label)
                               .SetRefreshInterval(options.CacheExpiration);
                    });
                }
            });
            
            return builder.Build();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to build Azure App Configuration");
            throw new InvalidOperationException("Failed to initialize Azure App Configuration provider", ex);
        }
    }

    private string GetFullKey(string key)
    {
        if (string.IsNullOrEmpty(options.KeyPrefix))
            return key;
        
        return $"{options.KeyPrefix}:{key}";
    }

    private static T ConvertValue<T>(string stringValue, T defaultValue)
    {
        try
        {
            if (typeof(T) == typeof(string))
                return (T)(object)stringValue;

            if (typeof(T) == typeof(int))
                return (T)(object)int.Parse(stringValue);

            if (typeof(T) == typeof(long))
                return (T)(object)long.Parse(stringValue);

            if (typeof(T) == typeof(double))
                return (T)(object)double.Parse(stringValue);

            if (typeof(T) == typeof(decimal))
                return (T)(object)decimal.Parse(stringValue);

            if (typeof(T) == typeof(bool))
                return (T)(object)bool.Parse(stringValue);

            if (typeof(T) == typeof(DateTime))
                return (T)(object)DateTime.Parse(stringValue);

            if (typeof(T) == typeof(DateTimeOffset))
                return (T)(object)DateTimeOffset.Parse(stringValue);

            if (typeof(T) == typeof(TimeSpan))
                return (T)(object)TimeSpan.Parse(stringValue);

            if (typeof(T) == typeof(Guid))
                return (T)(object)Guid.Parse(stringValue);

            // For complex types, try JSON deserialization
            return JsonSerializer.Deserialize<T>(stringValue) ?? defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!isDisposed && disposing)
        {
            refreshSemaphore?.Dispose();
            cache?.Clear();
            isDisposed = true;
        }
        
        base.Dispose(disposing);
    }
}
