using System.Reflection;

namespace VisionaryCoder.Framework;

/// <summary>
/// Default implementation of <see cref="IFrameworkInfoProvider"/>.
/// </summary>
public sealed class FrameworkInfoProvider : IFrameworkInfoProvider
{
    /// <inheritdoc />
    public string Version => FrameworkConstants.Version;

    /// <inheritdoc />
    public string Name => "VisionaryCoder Framework";

    /// <inheritdoc />
    public string Description => "A comprehensive framework for building enterprise-grade applications with proxy interceptor architecture.";

    /// <inheritdoc />
    public DateTimeOffset CompiledAt { get; } = GetCompilationTime();

    private static DateTimeOffset GetCompilationTime()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileInfo = new FileInfo(assembly.Location);
        return fileInfo.CreationTime;
    }
}