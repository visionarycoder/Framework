namespace VisionaryCoder.Framework.Proxy.Abstractions.Exceptions;

/// <summary>
/// Represents a business logic exception.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public class BusinessException(string message) : ProxyException(message)
{
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BusinessException(string message, Exception innerException) : base(message, innerException) { }
}
