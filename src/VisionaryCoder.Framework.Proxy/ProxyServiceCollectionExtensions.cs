using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;
using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy;

/// <summary>
/// Extension methods for configuring proxy pipeline services.
/// </summary>
public static class ProxyServiceCollectionExtensions
{
    /// <summary>
    /// Adds the default proxy pipeline.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddProxyPipeline(this IServiceCollection services)
    {
        // Register core pipeline components
        services.TryAddSingleton<IProxyPipeline, DefaultProxyPipeline>();

        // Register memory cache if not already registered
        services.TryAddSingleton<IMemoryCache, MemoryCache>();

        return services;
    }

    /// <summary>
    /// Adds a custom proxy transport implementation.
    /// </summary>
    /// <typeparam name="TTransport">The transport implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddProxyTransport<TTransport>(this IServiceCollection services)
        where TTransport : class, IProxyTransport
    {
        services.TryAddSingleton<IProxyTransport, TTransport>();
        return services;
    }

    /// <summary>
    /// Adds a custom interceptor to the proxy pipeline.
    /// </summary>
    /// <typeparam name="TInterceptor">The interceptor implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime for the interceptor.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddProxyInterceptor<TInterceptor>(
        this IServiceCollection services, 
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TInterceptor : class, IProxyInterceptor
    {
        services.TryAdd(ServiceDescriptor.Describe(typeof(TInterceptor), typeof(TInterceptor), lifetime));
        services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(IProxyInterceptor), typeof(TInterceptor), lifetime));
        return services;
    }
}

/// <summary>
/// Example HTTP transport implementation.
/// </summary>
public class HttpProxyTransport : IProxyTransport
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpProxyTransport"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for requests.</param>
    public HttpProxyTransport(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Sends an HTTP request and returns a typed response.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <returns>A task representing the HTTP response.</returns>
    public async Task<Response<T>> SendCoreAsync<T>(ProxyContext context)
    {
        try
        {
            var request = new HttpRequestMessage(new HttpMethod(context.Method), context.Url);
            
            // Add headers from context
            foreach (var header in context.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            
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