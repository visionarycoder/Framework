namespace VisionaryCoder.Proxy.Abstractions;

public abstract class ProxyException : Exception
{
    protected ProxyException(string message) : base(message) { }
    protected ProxyException(string message, Exception inner) : base(message, inner) { }
}