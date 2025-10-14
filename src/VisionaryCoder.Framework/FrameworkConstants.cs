namespace VisionaryCoder.Framework;

/// <summary>
/// Framework-wide constants for the VisionaryCoder Framework.
/// </summary>
public static class FrameworkConstants
{
    /// <summary>
    /// The current framework version.
    /// </summary>
    public const string Version = "1.0.0";

    /// <summary>
    /// The default configuration section name for framework settings.
    /// </summary>
    public const string ConfigurationSection = "VisionaryCoderFramework";

    /// <summary>
    /// Default timeout values for various operations.
    /// </summary>
    public static class Timeouts
    {
        /// <summary>
        /// Default HTTP client timeout in seconds.
        /// </summary>
        public const int DefaultHttpTimeoutSeconds = 30;

        /// <summary>
        /// Default database command timeout in seconds.
        /// </summary>
        public const int DefaultDatabaseTimeoutSeconds = 30;

        /// <summary>
        /// Default cache expiration in minutes.
        /// </summary>
        public const int DefaultCacheExpirationMinutes = 15;
    }

    /// <summary>
    /// Common header names used throughout the framework.
    /// </summary>
    public static class Headers
    {
        /// <summary>
        /// Correlation ID header name for request tracking.
        /// </summary>
        public const string CorrelationId = "X-Correlation-ID";

        /// <summary>
        /// Request ID header name for individual request tracking.
        /// </summary>
        public const string RequestId = "X-Request-ID";

        /// <summary>
        /// User context header name for user information.
        /// </summary>
        public const string UserContext = "X-User-Context";

        /// <summary>
        /// API version header name.
        /// </summary>
        public const string ApiVersion = "Api-Version";
    }

    /// <summary>
    /// Logging-related constants.
    /// </summary>
    public static class Logging
    {
        /// <summary>
        /// Default log message template for structured logging.
        /// </summary>
        public const string DefaultTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

        /// <summary>
        /// Correlation ID property name for structured logging.
        /// </summary>
        public const string CorrelationIdProperty = "CorrelationId";

        /// <summary>
        /// Request ID property name for structured logging.
        /// </summary>
        public const string RequestIdProperty = "RequestId";

        /// <summary>
        /// User ID property name for structured logging.
        /// </summary>
        public const string UserIdProperty = "UserId";
    }
}