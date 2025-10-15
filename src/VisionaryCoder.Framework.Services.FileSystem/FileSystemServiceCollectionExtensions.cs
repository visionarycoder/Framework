using Microsoft.Extensions.DependencyInjection;
using VisionaryCoder.Framework.Services.Abstractions;

namespace VisionaryCoder.Framework.Services.FileSystem;

/// <summary>
/// Extension methods for registering file system services with dependency injection.
/// Follows Microsoft best practices for service registration.
/// </summary>
public static class FileSystemServiceCollectionExtensions
{
    /// <summary>
    /// Registers the local file system services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddFileSystemServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Register the consolidated file system service
        services.AddScoped<IFileSystem, FileSystemService>();

        // Optionally register the individual services for backward compatibility
        services.AddScoped<IFileService>(provider => new FileService(
            provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<FileService>>()));
        
        return services;
    }

    /// <summary>
    /// Registers the local file system services as singletons with the dependency injection container.
    /// Use this when you want to share the same instance across the application lifetime.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    /// File system operations are generally safe to use as singletons since they don't maintain state.
    /// However, be aware that logging will be shared across all consumers.
    /// </remarks>
    public static IServiceCollection AddFileSystemServicesSingleton(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IFileSystem, FileSystemService>();
        
        return services;
    }

    /// <summary>
    /// Registers the FTP file system services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="options">The FTP configuration options.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddFtpFileSystemServices(this IServiceCollection services, FtpFileSystemOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services.AddSingleton(options);
        services.AddScoped<IFileSystem, FtpFileSystemService>();
        
        return services;
    }

    /// <summary>
    /// Registers the FTP file system services as a singleton with the dependency injection container.
    /// Use this when you want to share the same FTP connection configuration across the application.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="options">The FTP configuration options.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddFtpFileSystemServicesSingleton(this IServiceCollection services, FtpFileSystemOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services.AddSingleton(options);
        services.AddSingleton<IFileSystem, FtpFileSystemService>();
        
        return services;
    }

    /// <summary>
    /// Registers a named FTP file system service with the dependency injection container.
    /// This allows multiple FTP configurations to coexist.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="name">The name for this FTP service instance.</param>
    /// <param name="options">The FTP configuration options.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddNamedFtpFileSystemServices(this IServiceCollection services, string name, FtpFileSystemOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(options);

        services.AddKeyedScoped<IFileSystem, FtpFileSystemService>(name, (provider, key) => 
            new FtpFileSystemService(options, provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<FtpFileSystemService>>()));
        
        return services;
    }
}