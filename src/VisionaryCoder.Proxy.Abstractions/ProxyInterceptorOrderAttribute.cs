namespace VisionaryCoder.Proxy.Abstractions;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class ProxyInterceptorOrderAttribute(int order) : Attribute
{
    public int Order { get; } = order;
}