using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Caching.Memory;
using VisionaryCoder.Framework.Services.Abstractions;
using VisionaryCoder.Framework.Services.FileSystem;
using VisionaryCoder.Framework.Secrets.Abstractions;

namespace VisionaryCoder.Framework.Extensions.DependencyInjection;

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
        services.TryAddTransient<IFileSystem, FileSystemService>();
        return services;
    }

    /// <summary>
    /// Registers the FTP file system service implementation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">The FTP file system configuration options.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddFtpFileSystem(this IServiceCollection services, FtpFileSystemOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();

        services.AddSingleton(options);
        services.TryAddTransient<IFileSystem, FtpFileSystemService>();
        return services;
    }

    /// <summary>
    /// Registers the FTP file system service implementation with configuration delegate.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">The configuration delegate for FTP options.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddFtpFileSystem(this IServiceCollection services, Action<FtpFileSystemOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        var options = new FtpFileSystemOptions();
        configureOptions(options);
        options.Validate();

        return services.AddFtpFileSystem(options);
    }

    /// <summary>
    /// Registers the secure FTP file system service implementation.
    /// This service requires ISecretProvider to be registered separately.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">The secure FTP file system configuration options.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddSecureFtpFileSystem(this IServiceCollection services, SecureFtpFileSystemOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();

        // Ensure memory cache is available for credential caching
        services.AddMemoryCache();

        // Register the secure options
        services.AddSingleton(options);
        
        // Register the secure FTP service
        services.TryAddTransient<IFileSystem, SecureFtpFileSystemService>();
        
        return services;
    }

    /// <summary>
    /// Registers the secure FTP file system service implementation with configuration delegate.
    /// This service requires ISecretProvider to be registered separately.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">The configuration delegate for secure FTP options.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddSecureFtpFileSystem(this IServiceCollection services, Action<SecureFtpFileSystemOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        var options = new SecureFtpFileSystemOptions();
        configureOptions(options);
        options.Validate();

        return services.AddSecureFtpFileSystem(options);
    }

    /// <summary>
    /// Registers multiple file system implementations with a factory pattern.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>A file system registration builder for configuring multiple implementations.</returns>
    public static FileSystemRegistrationBuilder AddFileSystemFactory(this IServiceCollection services)
    {
        // Register factory interface
        services.TryAddTransient<IFileSystemFactory, FileSystemFactory>();
        return new FileSystemRegistrationBuilder(services);
    }

    /// <summary>
    /// Validates that required dependencies for secure file systems are registered.
    /// </summary>
    /// <param name="services">The service collection to validate.</param>
    /// <param name="requireSecretProvider">Whether to require ISecretProvider registration.</param>
    /// <exception cref="InvalidOperationException">Thrown when required dependencies are missing.</exception>
    public static void ValidateFileSystemDependencies(this IServiceCollection services, bool requireSecretProvider = false)
    {
        if (requireSecretProvider)
        {
            var hasSecretProvider = services.Any(s => s.ServiceType == typeof(ISecretProvider));
            if (!hasSecretProvider)
            {
                throw new InvalidOperationException(
                    "ISecretProvider is required for secure file system services. " +
                    "Please register a secret provider (e.g., KeyVaultSecretProvider) before adding secure file systems.");
            }
        }

        var hasMemoryCache = services.Any(s => s.ServiceType == typeof(IMemoryCache));
        if (!hasMemoryCache)
        {
            throw new InvalidOperationException(
                "IMemoryCache is required for file system services with caching support. " +
                "Please call services.AddMemoryCache() before registering file systems.");
        }
    }
}

/// <summary>
/// Builder for configuring multiple file system implementations.
/// </summary>
public sealed class FileSystemRegistrationBuilder
{
    private readonly IServiceCollection _services;

    internal FileSystemRegistrationBuilder(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    /// Adds a local file system implementation to the factory.
    /// </summary>
    /// <param name="name">The unique name for this file system implementation.</param>
    /// <returns>The builder for method chaining.</returns>
    public FileSystemRegistrationBuilder AddLocal(string name = "local")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        _services.Configure<FileSystemFactoryOptions>(options =>
            options.RegisterImplementation(name, typeof(FileSystemService)));

        _services.TryAddTransient<FileSystemService>();
        return this;
    }

    /// <summary>
    /// Adds an FTP file system implementation to the factory.
    /// </summary>
    /// <param name="name">The unique name for this file system implementation.</param>
    /// <param name="options">The FTP configuration options.</param>
    /// <returns>The builder for method chaining.</returns>
    public FileSystemRegistrationBuilder AddFtp(string name, FtpFileSystemOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(options);
        
        options.Validate();

        _services.Configure<FileSystemFactoryOptions>(factoryOptions =>
            factoryOptions.RegisterImplementation(name, typeof(FtpFileSystemService), options));

        _services.TryAddTransient<FtpFileSystemService>();
        return this;
    }

    /// <summary>
    /// Adds a secure FTP file system implementation to the factory.
    /// </summary>
    /// <param name="name">The unique name for this file system implementation.</param>
    /// <param name="options">The secure FTP configuration options.</param>
    /// <returns>The builder for method chaining.</returns>
    public FileSystemRegistrationBuilder AddSecureFtp(string name, SecureFtpFileSystemOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(options);
        
        options.Validate();

        // Ensure dependencies are available
        _services.AddMemoryCache();
        _services.ValidateFileSystemDependencies(requireSecretProvider: true);

        _services.Configure<FileSystemFactoryOptions>(factoryOptions =>
            factoryOptions.RegisterImplementation(name, typeof(SecureFtpFileSystemService), options));

        _services.TryAddTransient<SecureFtpFileSystemService>();
        return this;
    }
}

/// <summary>
/// Configuration options for the file system factory.
/// </summary>
public sealed class FileSystemFactoryOptions
{
    private readonly Dictionary<string, FileSystemImplementation> _implementations = new();

    /// <summary>
    /// Gets the registered file system implementations.
    /// </summary>
    public IReadOnlyDictionary<string, FileSystemImplementation> Implementations => _implementations;

    /// <summary>
    /// Registers a file system implementation.
    /// </summary>
    /// <param name="name">The unique name for this implementation.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="options">Optional configuration options for the implementation.</param>
    internal void RegisterImplementation(string name, Type implementationType, object? options = null)
    {
        _implementations[name] = new FileSystemImplementation(implementationType, options);
    }
}

/// <summary>
/// Represents a registered file system implementation.
/// </summary>
/// <param name="ImplementationType">The type of the file system implementation.</param>
/// <param name="Options">Optional configuration options for the implementation.</param>
public sealed record FileSystemImplementation(Type ImplementationType, object? Options = null);

/// <summary>
/// Factory interface for creating file system instances by name.
/// </summary>
public interface IFileSystemFactory
{
    /// <summary>
    /// Creates a file system instance by name.
    /// </summary>
    /// <param name="name">The registered name of the file system implementation.</param>
    /// <returns>The file system instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the specified name is not registered.</exception>
    IFileSystem Create(string name);

    /// <summary>
    /// Gets the names of all registered file system implementations.
    /// </summary>
    /// <returns>An enumerable of registered implementation names.</returns>
    IEnumerable<string> GetRegisteredNames();
}

/// <summary>
/// Default implementation of the file system factory.
/// </summary>
internal sealed class FileSystemFactory : IFileSystemFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly FileSystemFactoryOptions _options;

    public FileSystemFactory(IServiceProvider serviceProvider, Microsoft.Extensions.Options.IOptions<FileSystemFactoryOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    /// <inheritdoc />
    public IFileSystem Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!_options.Implementations.TryGetValue(name, out var implementation))
        {
            throw new ArgumentException($"File system implementation '{name}' is not registered. " +
                                      $"Available implementations: {string.Join(", ", GetRegisteredNames())}", nameof(name));
        }

        // Create instance using service provider
        var instance = ActivatorUtilities.CreateInstance(_serviceProvider, implementation.ImplementationType, implementation.Options ?? Array.Empty<object>());
        
        if (instance is not IFileSystem fileSystem)
        {
            throw new InvalidOperationException($"Implementation type '{implementation.ImplementationType.FullName}' does not implement IFileSystem");
        }

        return fileSystem;
    }

    /// <inheritdoc />
    public IEnumerable<string> GetRegisteredNames()
    {
        return _options.Implementations.Keys;
    }
}