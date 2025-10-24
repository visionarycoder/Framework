using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Caching.Memory;
using VisionaryCoder.Framework.Abstractions.Services;

namespace VisionaryCoder.Framework.Services.FileSystem;
/// <summary>
/// Extension methods for registering file system services with dependency injection.
/// </summary>
public static class FileSystemServiceExtensions
{
    /// <summary>
    /// Registers the local file system service implementation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddLocalFileSystem(this IServiceCollection services)
    {
    services.TryAddTransient<IFileSystemProvider, FileSystemService>();
        return services;
    }
    /// <summary>
    /// Registers the FluentFTP-based file system provider implementation.
    /// </summary>
    public static IServiceCollection AddFtpFileSystem(this IServiceCollection services, FtpFileSystemOptions options)
    {
        services.AddSingleton(options);
        services.TryAddTransient<IFileSystemProvider, FtpFileSystemProviderService>();
        return services;
    }
    // The overload for AddSecureFtpFileSystem with Action<SecureFtpFileSystemOptions> is disabled due to constructor limitations.
    // The AddFileSystemFactory method is disabled due to missing type definitions.
    // No validation helper needed for the basic FTP provider.
}
