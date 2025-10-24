using Microsoft.Extensions.Logging;

namespace VisionaryCoder.Framework.Abstractions;

/// <summary>
/// Base class for all framework services, providing common functionality like logging.
/// </summary>
/// <typeparam name="T">The concrete service type for typed logging.</typeparam>
public abstract class ServiceBase<T>(ILogger<T> logger) where T : class
{
    /// <summary>
    /// Gets the logger instance for this service.
    /// </summary>
    protected ILogger<T> Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));
}
