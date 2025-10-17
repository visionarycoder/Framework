using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace VisionaryCoder.Framework.Extensions.Configuration;

public sealed class KeyVaultSecretProvider(SecretClient client, IOptions<SecretOptions> opts, IMemoryCache cache)
    : ISecretProvider
{
    public async Task<string?> GetAsync(string name, CancellationToken cancellationToken = default)
    {
        var ttl = opts.Value.CacheTtl;
        if (cache.TryGetValue(name, out string? hit)) return hit;

        var secret = await client.GetSecretAsync(name, cancellationToken: cancellationToken);
        var value = secret.Value.Value;

        if (!string.IsNullOrEmpty(value))
            cache.Set(name, value, ttl);

        return value;
    }
}
