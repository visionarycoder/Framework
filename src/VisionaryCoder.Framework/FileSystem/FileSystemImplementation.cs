namespace VisionaryCoder.Framework.Services.FileSystem;

/// <summary>
/// Represents a registered file system implementation.
/// </summary>
/// <param name="ImplementationType">The type of the file system implementation.</param>
/// <param name="Options">Optional configuration options for the implementation.</param>
public sealed record FileSystemImplementation(Type ImplementationType, object? Options = null);
