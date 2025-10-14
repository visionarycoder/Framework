namespace VisionaryCoder.Framework.Extensions.Configuration;

public interface ISecretProvider
{
    Task<string?> GetAsync(string name, CancellationToken ct = default);
}
