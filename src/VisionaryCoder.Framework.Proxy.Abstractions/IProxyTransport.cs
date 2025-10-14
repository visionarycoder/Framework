using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Defines the contract for proxy transport implementations.
/// </summary>
public interface IProxyTransport
{
    /// <summary>
    /// Sends a request through the transport layer and returns a response.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    /// <param name="context">The proxy context containing request information.</param>
    /// <returns>A task representing the asynchronous operation with the response.</returns>
    Task<Response<T>> SendCoreAsync<T>(ProxyContext context);
}