using VisionaryCoder.Proxy.Abstractions;

namespace VisionaryCoder.Proxy.Interceptors;

public sealed class MemoryProxyCache(IMemoryCache cache) : IProxyCache
{
    public bool TryGetValue<T>(string key, out T value)
    {
        if (cache.TryGetValue(key, out var obj) && obj is T typed)
        {
            value = typed;
            return true;
        }
        value = default!;
        return false;
    }

    public void Set<T>(string key, T value, TimeSpan ttl)
        => cache.Set(key, value!, ttl);
}