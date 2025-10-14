using Microsoft.Extensions.Configuration;
using VisionaryCoder.Framework.Secrets.Abstractions;

namespace VisionaryCoder.Framework.Azure.KeyVault;

/// <summary>
/// Local implementation of ISecretProvider for development scenarios.
/// </summary>
public sealed class LocalSecretProvider : ISecretProvider
{
    private readonly IConfiguration _configuration;
    private readonly KeyVaultOptions _options;

    public LocalSecretProvider(IConfiguration configuration, KeyVaultOptions options)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Retrieves a secret from local configuration sources.
    /// Searches in order: Secrets:{name}, {name}, Environment Variable {name}
    /// </summary>
    public Task<string?> GetAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Secret name cannot be null or empty.", nameof(name));
        }

        // Try configuration with prefix first
        var prefixedKey = $"{_options.LocalSecretsPrefix}:{name}";
        var value = _configuration[prefixedKey] 
                   ?? _configuration[name] 
                   ?? Environment.GetEnvironmentVariable(name);

        return Task.FromResult(value);
    }
}