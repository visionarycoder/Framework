using Microsoft.Extensions.DependencyInjection;
using VisionaryCoder.Framework.Abstractions;
using VisionaryCoder.Framework.Storage.Ftp;

namespace VisionaryCoder.Framework.Storage;

/// <summary>
/// Extension methods for registering storage services with dependency injection.
/// </summary>
public static class StorageServiceCollectionExtensions
{
    /// <summary>
    /// Registers the local storage implementation.
    /// </summary>
    public static IServiceCollection AddLocalStorage(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddTransient<IStorageProvider, LocalStorageProvider>();
        return services;
    }

    /// <summary>
    /// Registers the FluentFTP-based storage implementation.
    /// </summary>
    public static IServiceCollection AddFtpStorage(this IServiceCollection services, FtpStorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);
        services.AddSingleton(options);
        services.AddTransient<IStorageProvider, FtpStorageProvider>();
        return services;
    }

    /// <summary>
    /// Registers a named FluentFTP-based storage implementation (requires .NET 8 keyed services).
    /// </summary>
    public static IServiceCollection AddNamedFtpStorage(this IServiceCollection services, string name, FtpStorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(options);
        services.AddSingleton(options);
        services.AddKeyedTransient<IStorageProvider, FtpStorageProvider>(name);
        return services;
    }
}