namespace VisionaryCoder.Proxy.Abstractions;

public sealed class NonRetryableTransportException : ProxyException
{
    public NonRetryableTransportException(string message) : base(message) { }
    public NonRetryableTransportException(string message, Exception inner) : base(message, inner) { }
}