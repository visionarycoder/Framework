namespace VisionaryCoder.Framework.Services.FileSystem;

using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// Builder for configuring multiple file system implementations.
/// </summary>
public sealed class FileSystemRegistrationBuilder
{
    private readonly IServiceCollection services;
    internal FileSystemRegistrationBuilder(IServiceCollection services)
    {
        this.services = services;
    }
    /// <summary>
    /// Adds a local file system implementation to the factory.
    /// </summary>
    /// <param name="name">The unique name for this file system implementation.</param>
    /// <returns>The builder for method chaining.</returns>
    public FileSystemRegistrationBuilder AddLocal(string name = "local")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        services.Configure<FileSystemFactoryOptions>(options =>
            options.RegisterImplementation(name, typeof(FileSystemService)));
        services.TryAddTransient<FileSystemService>();
        return this;
    }
    /// <summary>
    /// Adds an FTP/FTPS file system implementation to the factory using FluentFTP.
    /// </summary>
    /// <param name="name">The unique name for this file system implementation.</param>
    /// <param name="options">The FTP configuration options.</param>
    public FileSystemRegistrationBuilder AddFtp(string name, FtpFileSystemOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        services.Configure<FileSystemFactoryOptions>(factoryOptions =>
            factoryOptions.RegisterImplementation(name, typeof(FtpFileSystemProviderService), options));
        services.TryAddTransient<FtpFileSystemProviderService>();
        return this;
    }
}
