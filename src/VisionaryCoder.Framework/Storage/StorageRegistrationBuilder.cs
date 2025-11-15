using VisionaryCoder.Framework.Storage.Ftp;
using VisionaryCoder.Framework.Storage.Local;

namespace VisionaryCoder.Framework.Storage;

/// <summary>
/// Builder for configuring multiple storage implementations.
/// </summary>
public sealed class StorageRegistrationBuilder(IServiceCollection services)
{

    /// <summary>
    /// Adds a local storage implementation to the factory.
    /// </summary>
    /// <param name="name">The unique name for this storage implementation.</param>
    /// <returns>The builder for method chaining.</returns>
    public StorageRegistrationBuilder AddLocal(string name = "local")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        services.Configure<StorageFactoryOptions>(options => options.RegisterImplementation(name, typeof(LocalStorageProvider)));
        services.TryAddTransient<LocalStorageProvider>();
        return this;
    }

    /// <summary>
    /// Adds an FTP/FTPS storage implementation to the factory using FluentFTP.
    /// </summary>
    /// <param name="name">The unique name for this storage implementation.</param>
    /// <param name="options">The FTP configuration options.</param>
    public StorageRegistrationBuilder AddFtp(string name, FtpStorageOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        services.Configure<StorageFactoryOptions>(factoryOptions => factoryOptions.RegisterImplementation(name, typeof(FtpStorageProvider), options));
        services.TryAddTransient<FtpStorageProvider>();
        return this;
    }

}
