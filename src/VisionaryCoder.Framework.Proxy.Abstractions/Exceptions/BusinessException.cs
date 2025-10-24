namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Represents a business logic exception.
/// </summary>
public class BusinessException : ProxyException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BusinessException(string message) : base(message) { }
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BusinessException(string message, Exception innerException) : base(message, innerException) { }
}
