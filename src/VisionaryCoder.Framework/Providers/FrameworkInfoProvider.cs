using System.Reflection;
using VisionaryCoder.Framework.Abstractions;

namespace VisionaryCoder.Framework.Providers;

/// <summary>
/// Default implementation of <see cref="IFrameworkInfoProvider"/>.
/// </summary>
public sealed class FrameworkInfoProvider : IFrameworkInfoProvider
{
    /// <inheritdoc />
    public string Version
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                   ?? assembly.GetName().Version?.ToString() ?? "0.0.0";
        }
    }

    public string Name => "VisionaryCoder Framework";
    public string Description => "A comprehensive framework for building enterprise-grade applications with proxy interceptor architecture.";
    public DateTimeOffset CompiledAt { get; } = GetCompilationTime();

    private static DateTimeOffset GetCompilationTime()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileInfo = new FileInfo(assembly.Location);
        return fileInfo.CreationTime;
    }
}
