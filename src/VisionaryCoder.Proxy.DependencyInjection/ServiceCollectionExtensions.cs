using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;
using VisionaryCoder.Proxy.Abstractions;
using VisionaryCoder.Proxy.Interceptors;
using CachingInterceptor = VisionaryCoder.Proxy.Abstractions.CachingInterceptor;

namespace VisionaryCoder.Proxy.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProxy(this IServiceCollection services, Action<ProxyOptions>? configure = null)
    {
        var options = new ProxyOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<IProxyErrorClassifier, DefaultProxyErrorClassifier>();

        // Core infra
        services.AddSingleton(new ActivitySource("VisionaryCoder.Proxy"));
        services.AddMemoryCache();
        services.AddSingleton<IProxyCache, MemoryProxyCache>();

        // You provide these (or no-op impls) in your app:
        services.TryAddSingleton<ISecurityEnricher, NoopSecurityEnricher>();
        services.TryAddSingleton<IAuthorizationPolicy, AllowAllAuthorizationPolicy>();
        services.TryAddSingleton<IAuditSink, NoopAuditSink>();
        services.TryAddSingleton<ICacheKeyProvider, DefaultCacheKeyProvider>();
        services.TryAddSingleton<ICachePolicyProvider, DefaultCachePolicyProvider>();

        // Interceptors (ordered)
        services.AddProxyInterceptor<SecurityInterceptor>(order: -200);
        services.AddProxyInterceptor<TelemetryInterceptor>(order: -50);
        services.AddProxyInterceptor<CorrelationInterceptor>(order: 0);
        services.AddProxyInterceptor<LoggingInterceptor>(order: 100);
        services.AddProxyInterceptor<CachingInterceptor>(order: 150);
        services.AddProxyInterceptor<ResilienceInterceptor>(order: 180);
        services.AddProxyInterceptor<RetryInterceptor>(order: 200); // from earlier
        services.AddProxyInterceptor<AuditingInterceptor>(order: 300);

        // Transport + pipeline + client
        services.AddHttpClient<HttpProxyTransport>();
        services.AddSingleton<IProxyTransport, HttpProxyTransport>();
        services.AddSingleton<DefaultProxyPipeline>();
        services.AddTransient<IProxyClient, ProxyClient>();

        return services;
    }

    // Convenience when callers supply their own ordered interceptors
    public static IServiceCollection AddProxyInterceptor<T>(this IServiceCollection services, int order) where T : class, IProxyInterceptor
    {
        services.AddSingleton<T>();
        services.AddSingleton<IProxyInterceptor>(sp => new OrderedProxyInterceptor<T>(sp.GetRequiredService<T>(), order));
        return services;
    }

    // Minimal defaults
    private sealed class NoopSecurityEnricher : ISecurityEnricher
    {
        public Task EnrichAsync(ProxyContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class AllowAllAuthorizationPolicy : IAuthorizationPolicy
    {
        public Task<bool> AuthorizeAsync(ProxyContext context, CancellationToken cancellationToken = default) => Task.FromResult(true);
    }

    private sealed class NoopAuditSink : IAuditSink
    {
        public Task WriteAsync(AuditRecord record, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class DefaultCacheKeyProvider : ICacheKeyProvider
    {
        public string? GetKey(object request, Type resultType) => $"{request.GetType().FullName}:{resultType.FullName}:{System.Text.Json.JsonSerializer.Serialize(request)}";
    }

    private sealed class DefaultCachePolicyProvider : ICachePolicyProvider
    {
        public bool IsCacheable(object request, Type resultType) => true; // tighten in your app
        public TimeSpan? GetTtl(object request, Type resultType) => TimeSpan.FromMinutes(1);
    }
}