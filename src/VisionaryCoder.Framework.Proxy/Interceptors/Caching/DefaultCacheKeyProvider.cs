using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Caching;
/// <summary>
/// Default implementation of ICacheKeyProvider.
/// </summary>
public class DefaultCacheKeyProvider : ICacheKeyProvider
{
    /// <summary>
    /// Generates a cache key based on the operation name, URL, and method.
    /// </summary>
    /// <typeparam name="T">The response type.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <returns>A unique cache key.</returns>
    public string GenerateKey<T>(ProxyContext context)
    {
        var keyComponents = new List<string>
        {
            context.OperationName ?? "Unknown",
            context.Method,
            context.Url ?? string.Empty,
            typeof(T).Name
        };
        // Include relevant headers in the key
        if (context.Headers.Count > 0)
        {
            var headerString = string.Join(";", context.Headers
                .Where(h => IsRelevantHeader(h.Key))
                .OrderBy(h => h.Key)
                .Select(h => $"{h.Key}={h.Value}"));
            
            if (!string.IsNullOrEmpty(headerString))
            {
                keyComponents.Add(headerString);
            }
        }
        var combinedKey = string.Join("|", keyComponents);
        
        // Hash the key to ensure consistent length and avoid special characters
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(combinedKey));
        return Convert.ToBase64String(hashBytes);
    }
    /// <summary>
    /// Determines if a header should be included in the cache key.
    /// </summary>
    /// <param name="headerName">The header name.</param>
    /// <returns>True if the header should be included.</returns>
    private static bool IsRelevantHeader(string headerName)
    {
        var relevantHeaders = new[]
        {
            "Accept",
            "Accept-Language",
            "Content-Type",
            "X-API-Version"
        };
        return relevantHeaders.Any(h => h.Equals(headerName, StringComparison.OrdinalIgnoreCase));
    }
}
