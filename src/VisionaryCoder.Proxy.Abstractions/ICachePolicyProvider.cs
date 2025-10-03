namespace VisionaryCoder.Proxy.Abstractions;

public interface ICachePolicyProvider
{
    bool IsCacheable(object request, Type resultType);
    TimeSpan? GetTtl(object request, Type resultType);
}