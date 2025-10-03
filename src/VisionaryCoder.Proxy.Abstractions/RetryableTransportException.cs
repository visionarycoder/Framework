namespace VisionaryCoder.Proxy.Abstractions;

public sealed class RetryableTransportException : ProxyException
{
    public RetryableTransportException(string message) : base(message) { }
    public RetryableTransportException(string message, Exception inner) : base(message, inner) { }
}