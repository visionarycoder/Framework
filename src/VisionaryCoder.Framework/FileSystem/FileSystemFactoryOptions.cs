namespace VisionaryCoder.Framework.Services.FileSystem;

using System;
using System.Collections.Generic;

/// <summary>
/// Configuration options for the file system factory.
/// </summary>
public sealed class FileSystemFactoryOptions
{
    private readonly Dictionary<string, FileSystemImplementation> implementations = new();
    /// <summary>
    /// Gets the registered file system implementations.
    /// </summary>
    public IReadOnlyDictionary<string, FileSystemImplementation> Implementations => implementations;
    /// <summary>
    /// Registers a file system implementation.
    /// </summary>
    /// <param name="name">The unique name for this implementation.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="options">Optional configuration options for the implementation.</param>
    internal void RegisterImplementation(string name, Type implementationType, object? options = null)
    {
        implementations[name] = new FileSystemImplementation(implementationType, options);
    }
}
