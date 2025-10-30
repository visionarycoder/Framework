using System.Collections.Concurrent;
using System.Text.Json;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;

using VisionaryCoder.Framework.AppConfiguration;

namespace VisionaryCoder.Framework.AppConfiguration.Local;

/// <summary>
/// Provides local file-based configuration operations following Microsoft configuration patterns.
/// This service wraps local JSON configuration files with logging, error handling, caching, and async support.
/// Supports file watching for automatic reloading and multiple configuration file sources.
/// </summary>
public sealed class LocalAppConfigurationProvider : ServiceBase<LocalAppConfigurationProvider>, IAppConfigurationProvider
{
    private readonly LocalAppConfigurationProviderOptions options;
    private readonly IConfiguration configuration;
    private readonly ConcurrentDictionary<string, (object? Value, DateTimeOffset CachedAt)> cache;
    private readonly SemaphoreSlim refreshSemaphore;
    private readonly FileSystemWatcher? fileWatcher;
    private DateTimeOffset lastRefresh;
    private bool isDisposed;

    public LocalAppConfigurationProvider(
        LocalAppConfigurationProviderOptions options, 
        ILogger<LocalAppConfigurationProvider> logger)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        
        this.options = options;
        this.options.Validate();
        
        this.cache = new ConcurrentDictionary<string, (object?, DateTimeOffset)>();
        this.refreshSemaphore = new SemaphoreSlim(1, 1);
        this.lastRefresh = DateTimeOffset.UtcNow;
        
        this.configuration = BuildConfiguration();
        
        // Set up file watcher if reload on change is enabled
        if (options.ReloadOnChange)
        {
            this.fileWatcher = SetupFileWatcher();
        }
        
        Logger.LogInformation(
            "Local App Configuration provider initialized for file {FilePath} with {AdditionalFileCount} additional files",
            options.FilePath, 
            options.AdditionalFiles.Count());
    }

    public string ProviderName => "Local";

    public bool IsAvailable
    {
        get
        {
            try
            {
                bool mainFileExists = File.Exists(GetFullPath(options.FilePath));
                return options.Optional || mainFileExists;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Local configuration health check failed");
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
            
            // Check cache first if caching is enabled
            if (options.EnableCaching && TryGetFromCache<T>(fullKey, out T cachedValue))
            {
                Logger.LogTrace("Configuration value retrieved from cache for key {Key}", key);
                return cachedValue;
            }

            // Get from configuration
            string? stringValue = configuration[fullKey];
            if (string.IsNullOrEmpty(stringValue))
            {
                Logger.LogDebug("Configuration key {Key} not found, returning default value", key);
                return defaultValue;
            }

            T result = AppConfigurationHelper.ConvertValue<T>(stringValue, defaultValue);
            
            // Cache the result if caching is enabled
            if (options.EnableCaching)
            {
                cache.TryAdd(fullKey, (result, DateTimeOffset.UtcNow));
            }
            
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
        // Local file operations are typically fast, but we provide async wrapper for consistency
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
        Logger.LogWarning("SetValue operation not supported by Local App Configuration provider. Modify the configuration files directly.");
        throw new NotSupportedException("Local App Configuration provider is read-only. Modify the configuration files directly.");
    }

    public Task<bool> SetValueAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SetValue(key, value));
    }

    public bool UpdateSection<T>(string sectionName, T value) where T : class
    {
        Logger.LogWarning("UpdateSection operation not supported by Local App Configuration provider. Modify the configuration files directly.");
        throw new NotSupportedException("Local App Configuration provider is read-only. Modify the configuration files directly.");
    }

    public Task<bool> UpdateSectionAsync<T>(string sectionName, T value, CancellationToken cancellationToken = default) where T : class
    {
        return Task.FromResult(UpdateSection(sectionName, value));
    }

    public bool Refresh()
    {
        try
        {
            if (refreshSemaphore.Wait(TimeSpan.FromSeconds(5)))
            {
                try
                {
                    // Clear cache to force refresh
                    if (options.EnableCaching)
                    {
                        cache.Clear();
                    }
                    
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

    public async Task<bool> RefreshAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (await refreshSemaphore.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken))
            {
                try
                {
                    // Clear cache to force refresh
                    if (options.EnableCaching)
                    {
                        cache.Clear();
                    }
                    
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
        
        // Set base path if provided
        if (!string.IsNullOrEmpty(options.BasePath))
        {
            builder.SetBasePath(options.BasePath);
        }
        
        try
        {
            // Add main configuration file
            builder.AddJsonFile(options.FilePath, optional: options.Optional, reloadOnChange: options.ReloadOnChange);
            
            // Add additional configuration files
            foreach (string additionalFile in options.AdditionalFiles)
            {
                builder.AddJsonFile(additionalFile, optional: true, reloadOnChange: options.ReloadOnChange);
            }
            
            // Add environment variables as fallback
            builder.AddEnvironmentVariables();
            
            return builder.Build();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to build local configuration");
            throw new InvalidOperationException("Failed to initialize Local App Configuration provider", ex);
        }
    }

    private FileSystemWatcher? SetupFileWatcher()
    {
        try
        {
            string filePath = GetFullPath(options.FilePath);
            string? directory = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            
            if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(fileName))
                return null;
            
            if (!Directory.Exists(directory))
            {
                Logger.LogWarning("Directory {Directory} does not exist, file watcher will not be set up", directory);
                return null;
            }
            
            var watcher = new FileSystemWatcher(directory, fileName)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true
            };
            
            watcher.Changed += OnConfigurationFileChanged;
            
            Logger.LogDebug("File watcher set up for {FilePath}", filePath);
            return watcher;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to set up file watcher for {FilePath}", options.FilePath);
            return null;
        }
    }

    private void OnConfigurationFileChanged(object sender, FileSystemEventArgs e)
    {
        Logger.LogDebug("Configuration file {FilePath} changed, clearing cache", e.FullPath);
        
        // Clear cache when file changes
        if (options.EnableCaching)
        {
            cache.Clear();
        }
        
        lastRefresh = DateTimeOffset.UtcNow;
    }

    private string GetFullPath(string filePath)
    {
        if (Path.IsPathRooted(filePath))
            return filePath;
        
        if (!string.IsNullOrEmpty(options.BasePath))
            return Path.Combine(options.BasePath, filePath);
        
        return Path.Combine(Directory.GetCurrentDirectory(), filePath);
    }

    private string GetFullKey(string key)
    {
        if (string.IsNullOrEmpty(options.KeyPrefix))
            return key;
        
        return $"{options.KeyPrefix}:{key}";
    }

    private bool TryGetFromCache<T>(string key, out T value)
    {
        value = default!;
        
        if (!options.EnableCaching)
            return false;
            
        if (!cache.TryGetValue(key, out (object? Value, DateTimeOffset CachedAt) cached))
            return false;
        
        // Check if cache entry is expired
        if (DateTimeOffset.UtcNow - cached.CachedAt > options.CacheExpiration)
        {
            cache.TryRemove(key, out _);
            return false;
        }
        
        if (cached.Value is T typedValue)
        {
            value = typedValue;
            return true;
        }
        
        return false;
    }
    
    protected override void Dispose(bool disposing)
    {
        if (!isDisposed && disposing)
        {
            fileWatcher?.Dispose();
            refreshSemaphore?.Dispose();
            cache?.Clear();
            isDisposed = true;
        }
        
        base.Dispose(disposing);
    }
}
