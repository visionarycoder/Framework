namespace VisionaryCoder.Proxy.Abstractions;

public interface IProxyCache
{
    bool TryGetValue<T>(string key, out T value);
    void Set<T>(string key, T value, TimeSpan ttl);
}