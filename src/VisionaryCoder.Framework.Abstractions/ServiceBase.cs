using Microsoft.Extensions.Logging;

namespace VisionaryCoder.Framework.Abstractions;

/// <summary>
/// Provides a base class for services following Microsoft patterns with dependency injection support.
/// This class implements common service functionality including logging, instance identification, and lifecycle management.
/// </summary>
/// <typeparam name="T">The type of the service implementation for strongly-typed logging.</typeparam>
public abstract class ServiceBase<T>
{
    /// <summary>
    /// Gets the logger instance for the service.
    /// </summary>
    protected ILogger<T> Logger { get; }

    /// <summary>
    /// Gets the unique identifier for this service instance.
    /// </summary>
    public Guid InstanceId { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets the timestamp when this service instance was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBase{T}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for this service.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is null.</exception>
    protected ServiceBase(ILogger<T> logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Logger.LogDebug("Service {ServiceType} created with instance ID {InstanceId}", typeof(T).Name, InstanceId);
    }
}
