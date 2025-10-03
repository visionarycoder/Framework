namespace VisionaryCoder.Proxy.Abstractions;

public sealed class BusinessException : ProxyException
{
    public BusinessException(string message) : base(message) { }
    public BusinessException(string message, Exception inner) : base(message, inner) { }
}