using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

// ReSharper disable UnusedType.Global

namespace vc.Ifx.Services.Configuration;

public class JsonFileConfigurationService : ServiceBase, IConfigurationService
{

    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };

    private Dictionary<string, object> configuration = new();
    private bool isDirty;

    public string ConfigurationFilePath { get; }

    public JsonFileConfigurationService(string filePath = "AppSettings.json")
    {
        this.ConfigurationFilePath = filePath;
        Reload();
    }

    public T GetSection<T>(string sectionName) where T : class, new()
    {
        if (string.IsNullOrEmpty(sectionName))
        {
            // Return the entire configuration
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(configuration))!;
        }
        return configuration.TryGetValue(sectionName, out var section)
            ? JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(section)) ?? new T()
            : new T();
    }

    public async Task<T> GetSectionAsync<T>(string sectionName) where T : class, new()
    {
        return await Task.FromResult(GetSection<T>(sectionName));
    }

    public T GetValue<T>(string key, T defaultValue = default!)
    {
        return configuration.TryGetValue(key, out var value)
            ? JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value))!
            : defaultValue;
    }

    public IDictionary<string, object?> GetAllValues()
    {
        return new Dictionary<string, object?>(configuration);
    }

    public bool UpdateSection<T>(string sectionName, T value) where T : class
    {

        if (string.IsNullOrEmpty(sectionName))
        {
            throw new ArgumentException("Section name cannot be null or empty", nameof(sectionName));
        }
        try
        {
            configuration[sectionName] = value;
            isDirty = true;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateSectionAsync<T>(string sectionName, T value) where T : class
    {
        return await Task.FromResult(UpdateSection(sectionName, value));
    }

    public bool SetValue<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }
        try
        {
            configuration[key] = value;
            isDirty = true;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool RemoveSection(string sectionName)
    {
        if (string.IsNullOrEmpty(sectionName))
        {
            throw new ArgumentException("Section name cannot be null or empty", nameof(sectionName));
        }
        if (configuration.Remove(sectionName))
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    public bool RemoveValue(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }
        if (configuration.Remove(key))
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    public bool SaveChanges()
    {
        if (!isDirty)
        {
            return true;
        }
        try
        {
            var json = JsonSerializer.Serialize(configuration, jsonOptions);
            File.WriteAllText(ConfigurationFilePath, json);
            isDirty = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SaveChangesAsync()
    {
        if (!isDirty)
        {
            return true;
        }
        try
        {
            var json = JsonSerializer.Serialize(configuration, jsonOptions);
            await File.WriteAllTextAsync(ConfigurationFilePath, json);
            isDirty = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool Reload()
    {
        try
        {
            if (File.Exists(ConfigurationFilePath))
            {
                var json = File.ReadAllText(ConfigurationFilePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    configuration = JsonSerializer.Deserialize<Dictionary<string, object?>>(json, jsonOptions) ?? new Dictionary<string, object?>();
                    return true;
                }
            }
            configuration = new Dictionary<string, object?>();
            return true;
        }
        catch
        {
            configuration = new Dictionary<string, object?>();
            return false;
        }
    }

    public async Task<bool> ReloadAsync()
    {
        try
        {
            if (File.Exists(ConfigurationFilePath))
            {
                var json = await File.ReadAllTextAsync(ConfigurationFilePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    configuration = JsonSerializer.Deserialize<Dictionary<string, object?>>(json, jsonOptions) ?? new Dictionary<string, object?>();
                    return true;
                }
            }
            configuration = new Dictionary<string, object?>();
            return true;
        }
        catch
        {
            configuration = new Dictionary<string, object?>();
            return false;
        }
    }
}