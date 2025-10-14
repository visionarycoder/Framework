// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace VisionaryCoder.Framework.Proxy.Abstractions;

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