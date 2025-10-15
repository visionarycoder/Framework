namespace VisionaryCoder.Framework;

/// <summary>
/// Provides information about the VisionaryCoder Framework.
/// </summary>
public interface IFrameworkInfoProvider
{
    /// <summary>
    /// Gets the current framework version.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets the framework name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the framework description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets when the framework was compiled.
    /// </summary>
    DateTimeOffset CompiledAt { get; }
}