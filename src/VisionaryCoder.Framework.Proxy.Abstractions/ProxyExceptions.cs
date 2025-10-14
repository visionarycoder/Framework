// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace VisionaryCoder.Framework.Proxy.Abstractions.Exceptions;

/// <summary>
/// Base exception for proxy-related errors.
/// </summary>
public abstract class ProxyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyException"/> class.
    /// </summary>
    protected ProxyException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected ProxyException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected ProxyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when a proxy operation times out.
/// </summary>
public class ProxyTimeoutException : ProxyException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyTimeoutException"/> class.
    /// </summary>
    public ProxyTimeoutException() : base("The proxy operation timed out.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyTimeoutException"/> class with a specified timeout.
    /// </summary>
    /// <param name="timeout">The timeout that was exceeded.</param>
    public ProxyTimeoutException(TimeSpan timeout) : base($"The proxy operation timed out after {timeout}.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyTimeoutException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ProxyTimeoutException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyTimeoutException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ProxyTimeoutException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when a proxy operation fails due to a transient error.
/// </summary>
public class TransientProxyException : ProxyException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransientProxyException"/> class.
    /// </summary>
    public TransientProxyException() : base("A transient proxy error occurred.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransientProxyException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TransientProxyException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransientProxyException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public TransientProxyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

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

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BusinessException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Represents a transport exception that can be retried.
/// </summary>
public class RetryableTransportException : ProxyException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RetryableTransportException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RetryableTransportException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RetryableTransportException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RetryableTransportException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Represents a transport exception that cannot be retried.
/// </summary>
public class NonRetryableTransportException : ProxyException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NonRetryableTransportException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NonRetryableTransportException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonRetryableTransportException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NonRetryableTransportException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Represents an exception that occurs when a proxy operation is canceled.
/// </summary>
public class ProxyCanceledException : ProxyException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyCanceledException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ProxyCanceledException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyCanceledException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ProxyCanceledException(string message, Exception innerException) : base(message, innerException) { }
}