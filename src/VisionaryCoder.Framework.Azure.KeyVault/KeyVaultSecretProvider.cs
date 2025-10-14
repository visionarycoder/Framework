using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VisionaryCoder.Framework.Secrets.Abstractions;

namespace VisionaryCoder.Framework.Azure.KeyVault;

/// <summary>
/// Azure Key Vault implementation of ISecretProvider with caching support.
/// </summary>
public sealed class KeyVaultSecretProvider : ISecretProvider
{
    private readonly SecretClient client;
    private readonly IMemoryCache cache;
    private readonly ILogger<KeyVaultSecretProvider> logger;
    private readonly KeyVaultOptions options;

    public KeyVaultSecretProvider(
        SecretClient client,
        IOptions<KeyVaultOptions> options,
        IMemoryCache cache,
        ILogger<KeyVaultSecretProvider> logger)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Retrieves a secret from Azure Key Vault with caching support.
    /// </summary>
    public async Task<string?> GetAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Secret name cannot be null or empty.", nameof(name));
        }

        var cacheKey = $"secret:{name}";

        // Try cache first
        if (cache.TryGetValue(cacheKey, out string? cachedValue))
        {
            logger.LogDebug("Secret '{SecretName}' retrieved from cache", name);
            return cachedValue;
        }

        try
        {
            logger.LogDebug("Retrieving secret '{SecretName}' from Key Vault", name);
            
            var response = await client.GetSecretAsync(name, cancellationToken: cancellationToken);
            var value = response.Value?.Value;

            if (!string.IsNullOrEmpty(value))
            {
                // Cache the secret with configured TTL
                cache.Set(cacheKey, value, options.CacheTtl);
                logger.LogDebug("Secret '{SecretName}' cached for {CacheTtl}", name, options.CacheTtl);
            }
            else
            {
                logger.LogWarning("Secret '{SecretName}' returned empty value from Key Vault", name);
            }

            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve secret '{SecretName}' from Key Vault", name);
            
            // Don't cache failures, but don't rethrow to allow fallback behavior
            return null;
        }
    }

    /// <summary>
    /// Retrieves multiple secrets efficiently with parallel execution and caching.
    /// </summary>
    public async Task<IDictionary<string, string?>> GetMultipleAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        var secretNames = names?.ToList() ?? throw new ArgumentNullException(nameof(names));
        
        if (!secretNames.Any())
        {
            return new Dictionary<string, string?>();
        }

        logger.LogDebug("Retrieving {SecretCount} secrets from Key Vault", secretNames.Count);

        var tasks = secretNames.Select(async name =>
        {
            var value = await GetAsync(name, cancellationToken);
            return new { Name = name, Value = value };
        });

        var results = await Task.WhenAll(tasks);
        
        return results.ToDictionary(r => r.Name, r => r.Value);
    }
}