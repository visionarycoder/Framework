namespace VisionaryCoder.Framework;

/// <summary>
/// Configuration options for the VisionaryCoder Framework.
/// </summary>
public sealed class FrameworkOptions
{
    /// <summary>
    /// Gets or sets whether correlation ID generation is enabled.
    /// </summary>
    public bool EnableCorrelationId { get; set; } = true;

    /// <summary>
    /// Gets or sets whether request ID generation is enabled.
    /// </summary>
    public bool EnableRequestId { get; set; } = true;

    /// <summary>
    /// Gets or sets whether structured logging is enabled.
    /// </summary>
    public bool EnableStructuredLogging { get; set; } = true;

    /// <summary>
    /// Gets or sets the default HTTP timeout in seconds.
    /// </summary>
    public int DefaultHttpTimeoutSeconds { get; set; } = FrameworkConstants.Timeouts.DefaultHttpTimeoutSeconds;

    /// <summary>
    /// Gets or sets the default cache expiration in minutes.
    /// </summary>
    public int DefaultCacheExpirationMinutes { get; set; } = FrameworkConstants.Timeouts.DefaultCacheExpirationMinutes;
}