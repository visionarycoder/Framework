using VisionaryCoder.Framework.Providers;

namespace VisionaryCoder.Framework.Extensions;
/// <summary>
/// Extension methods for configuring the VisionaryCoder Framework services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the VisionaryCoder Framework services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddVisionaryCoderFramework(this IServiceCollection services)
    {
        return services.AddVisionaryCoderFramework(_ => { });
    }

    /// <summary>
    /// Adds the VisionaryCoder Framework services to the service collection with configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configureOptions">Action to configure framework options.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddVisionaryCoderFramework(
        this IServiceCollection services,
        Action<FrameworkOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);
        // Configure framework options
        services.Configure(configureOptions);
        // Register core framework services
        services.AddSingleton<IFrameworkInfoProvider, FrameworkInfoProvider>();
        services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
        // Framework services are now registered
        return services;
    }

    /// <summary>
    /// Adds framework correlation ID generation services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddFrameworkCorrelation(this IServiceCollection services)
    {
        services.AddScoped<IRequestIdProvider, RequestIdProvider>();
        return services;
    }
}
