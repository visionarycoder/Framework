namespace VisionaryCoder.Proxy.Abstractions;

/// Optional interface for code-based order (overrides attribute when both exist)
public interface IOrderedProxyInterceptor
{
    int Order { get; }
}