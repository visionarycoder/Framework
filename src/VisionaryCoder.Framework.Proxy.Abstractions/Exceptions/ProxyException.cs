using System;

namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Base exception type for all proxy-related errors.
/// </summary>
public class ProxyException : Exception
{
    public ProxyException() { }
    public ProxyException(string message) : base(message) { }
    public ProxyException(string message, Exception innerException) : base(message, innerException) { }
}
