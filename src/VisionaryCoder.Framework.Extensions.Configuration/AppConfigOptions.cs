namespace VisionaryCoder.Framework.Extensions.Configuration;

public sealed record AppConfigOptions
{
    public Uri? Endpoint { get; init; }                  // e.g., https://your-config.azconfig.io
    public string Label { get; init; } = "Production";   // use labels per env: Dev/Test/Prod
    public string SentinelKey { get; init; } = "App:Sentinel";
    public TimeSpan CacheExpiration { get; init; } = TimeSpan.FromSeconds(30);
}
