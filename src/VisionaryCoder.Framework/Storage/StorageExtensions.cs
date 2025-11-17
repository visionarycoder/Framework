using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VisionaryCoder.Framework.Storage.Azure.Blob;
using VisionaryCoder.Framework.Storage.Ftp;
using VisionaryCoder.Framework.Storage.Local;

namespace VisionaryCoder.Framework.Storage;

/// <summary>
/// Extension methods for registering storage services with dependency injection.
/// </summary>
public static class StorageExtensions
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
        services.TryAddTransient<IStorageProvider, FtpStorageProvider>();
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
        services.TryAddKeyedTransient<IStorageProvider, FtpStorageProvider>(name);
        return services;
    }

    /// <summary>
    /// Registers the Azure Blob storage provider implementation.
    /// </summary>
    public static IServiceCollection AddAzureBlobStorage(this IServiceCollection services, AzureBlobStorageOptions options)
    {
        services.AddSingleton(options);
        services.TryAddTransient<IStorageProvider, AzureBlobStorageProvider>();
        return services;
    }

    /// <summary>
    /// Registers the Azure Blob storage provider implementation.
    /// </summary>
    public static IServiceCollection AddAzureBlobStorage(this IServiceCollection services, string name, AzureBlobStorageOptions options)
    {
        services.AddSingleton(options);
        services.TryAddKeyedTransient<IStorageProvider, AzureBlobStorageProvider>(name);
        return services;
    }

}
