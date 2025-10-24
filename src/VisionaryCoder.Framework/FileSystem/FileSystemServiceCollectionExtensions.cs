using Microsoft.Extensions.DependencyInjection;
using VisionaryCoder.Framework.Abstractions.Services;

namespace VisionaryCoder.Framework.Services.FileSystem;

/// <summary>
/// Extension methods for registering file system services with dependency injection.
/// </summary>
public static class FileSystemServiceCollectionExtensions
{
    /// <summary>
    /// Registers the local file system implementation.
    /// </summary>
    public static IServiceCollection AddLocalFileSystem(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddTransient<IFileSystemProvider, FileSystemService>();
        return services;
    }

    /// <summary>
    /// Registers the FluentFTP-based file system implementation.
    /// </summary>
    public static IServiceCollection AddFtpFileSystem(this IServiceCollection services, FtpFileSystemOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);
        services.AddSingleton(options);
        services.AddTransient<IFileSystemProvider, FtpFileSystemProviderService>();
        return services;
    }

    /// <summary>
    /// Registers a named FluentFTP-based file system implementation (requires .NET 8 keyed services).
    /// </summary>
    public static IServiceCollection AddNamedFtpFileSystem(this IServiceCollection services, string name, FtpFileSystemOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(options);
        services.AddSingleton(options);
#if NET8_0_OR_GREATER
        services.AddKeyedTransient<IFileSystemProvider, FtpFileSystemProviderService>(name);
#endif
        return services;
    }
}
