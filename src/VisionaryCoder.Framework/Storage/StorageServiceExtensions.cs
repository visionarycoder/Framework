using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using VisionaryCoder.Framework.Storage.Azure;
using VisionaryCoder.Framework.Storage.Ftp;
using VisionaryCoder.Framework.Storage.Local;

namespace VisionaryCoder.Framework.Storage;
/// <summary>
/// Extension methods for registering storage services with dependency injection.
/// </summary>
public static class StorageServiceExtensions
{
    /// <summary>
    /// Registers the local storage service implementation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddLocalStorage(this IServiceCollection services)
    {
        services.TryAddTransient<IStorageProvider, LocalStorageProvider>();
        return services;
    }

    /// <summary>
    /// Registers the FluentFTP-based storage provider implementation.
    /// </summary>
    public static IServiceCollection AddFtpStorage(this IServiceCollection services, FtpStorageOptions options)
    {
        services.AddSingleton(options);
        services.TryAddTransient<IStorageProvider, FtpStorageProvider>();
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

}