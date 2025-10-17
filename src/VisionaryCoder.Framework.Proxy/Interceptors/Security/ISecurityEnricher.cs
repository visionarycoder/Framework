using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Interface for security enrichers that add security-related data to proxy contexts.
/// </summary>
public interface ISecurityEnricher
{
    /// <summary>
    /// Enriches the proxy context with security-related information.
    /// </summary>
    /// <param name="context">The proxy context to enrich.</param>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous enrichment operation.</returns>
    Task EnrichAsync(ProxyContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the order of execution for this enricher.
    /// </summary>
    int Order { get; }
}