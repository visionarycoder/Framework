using Microsoft.Extensions.Configuration;

namespace VisionaryCoder.Framework.Extensions.Configuration;

public sealed class LocalSecretProvider(IConfiguration config) : ISecretProvider
{
    public Task<string?> GetAsync(string name, CancellationToken ct = default)
        => Task.FromResult(config[$"Secrets:{name}"] ?? config[name] ?? Environment.GetEnvironmentVariable(name));
}
