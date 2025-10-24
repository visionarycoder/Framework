using System.Text.Json;
using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy;
/// <summary>
/// Example HTTP transport implementation.
/// </summary>
/// <param name="httpClient">The HTTP client to use for transport.</param>
internal sealed class HttpProxyTransport(HttpClient httpClient) : IProxyTransport
{
    private readonly HttpClient httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    /// <summary>
    /// Sends an HTTP request and returns a typed response.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the HTTP response.</returns>
    public async Task<Response<T>> SendCoreAsync<T>(ProxyContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(new HttpMethod(context.Method ?? "GET"), context.Url);
            
            // Add headers from context
            foreach (var header in context.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            var response = await httpClient.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<T>(content);
                return Response<T>.Success(data!, (int)response.StatusCode);
            }
            else
            {
                return Response<T>.Failure($"HTTP {response.StatusCode}: {content}");
            }
        }
        catch (Exception ex)
        {
            return Response<T>.Failure($"Transport error: {ex.Message}");
        }
    }
}
