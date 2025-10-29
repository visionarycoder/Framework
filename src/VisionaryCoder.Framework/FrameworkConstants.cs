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
    /// The default configuration section name for framework settings.
    public const string ConfigurationSection = "VisionaryCoderFramework";

    /// Default timeout values for various operations.
    public static class Timeouts
    {
        /// <summary>
        /// Default HTTP client timeout in seconds.
        /// </summary>
        public const int DefaultHttpTimeoutSeconds = 30;
        /// Default database command timeout in seconds.
        public const int DefaultDatabaseTimeoutSeconds = 30;
        /// Default cache expiration in minutes.
        public const int DefaultCacheExpirationMinutes = 15;
    }

    /// Common header names used throughout the framework.
    public static class Headers
    {
        /// Correlation ID header name for request tracking.
        public const string CorrelationId = "X-Correlation-ID";
        /// Request ID header name for individual request tracking.
        public const string RequestId = "X-Request-ID";
        /// User context header name for user information.
        public const string UserContext = "X-User-Context";
        /// API version header name.
        public const string ApiVersion = "Api-Version";
    }

    /// Logging-related constants.
    public static class Logging
    {
        /// Default log message template for structured logging.
        public const string DefaultTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";
        /// Correlation ID property name for structured logging.
        public const string CorrelationIdProperty = "CorrelationId";
        /// Request ID property name for structured logging.
        public const string RequestIdProperty = "RequestId";
        /// User ID property name for structured logging.
        public const string UserIdProperty = "UserId";
    }

}