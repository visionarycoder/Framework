using VisionaryCoder.Framework.Secrets.Abstractions;

namespace VisionaryCoder.Framework.Services.FileSystem;

/// <summary>
/// Secure configuration options for FTP file system operations that supports secret management.
/// This class integrates with ISecretProvider for secure credential retrieval.
/// </summary>
public sealed class SecureFtpFileSystemOptions
{
    /// <summary>
    /// Gets or sets the FTP server host address.
    /// </summary>
    public required string Host { get; init; }

    /// <summary>
    /// Gets or sets the FTP server port. Default is 21 for FTP, 990 for FTPS.
    /// </summary>
    public int Port { get; init; } = 21;

    /// <summary>
    /// Gets or sets the username for FTP authentication.
    /// Can be a direct value or a secret reference (e.g., "secret:ftp-username").
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Gets or sets the password for FTP authentication.
    /// Can be a direct value or a secret reference (e.g., "secret:ftp-password").
    /// When using secret references, the actual password will be retrieved from ISecretProvider.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// Gets or sets whether to use SSL/TLS for secure FTP (FTPS).
    /// </summary>
    public bool UseSsl { get; init; } = false;

    /// <summary>
    /// Gets or sets whether to use passive mode for FTP connections.
    /// </summary>
    public bool UsePassive { get; init; } = true;

    /// <summary>
    /// Gets or sets the timeout for FTP operations in milliseconds.
    /// </summary>
    public int TimeoutMilliseconds { get; init; } = 30000;

    /// <summary>
    /// Gets or sets the keep-alive interval for FTP connections.
    /// </summary>
    public bool KeepAlive { get; init; } = false;

    /// <summary>
    /// Gets or sets whether to use binary transfer mode.
    /// </summary>
    public bool UseBinary { get; init; } = true;

    /// <summary>
    /// Gets or sets the buffer size for file transfers.
    /// </summary>
    public int BufferSize { get; init; } = 8192;

    /// <summary>
    /// Gets or sets whether credentials should be cached after first retrieval from secret provider.
    /// Default is true for performance, but set to false for maximum security.
    /// </summary>
    public bool CacheCredentials { get; init; } = true;

    /// <summary>
    /// Gets or sets the cache duration for credentials when CacheCredentials is true.
    /// Default is 15 minutes.
    /// </summary>
    public TimeSpan CredentialCacheDuration { get; init; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Gets the FTP server URI based on the configuration.
    /// </summary>
    public string ServerUri => UseSsl ? $"ftps://{Host}:{Port}" : $"ftp://{Host}:{Port}";

    /// <summary>
    /// Determines if the username is a secret reference.
    /// </summary>
    public bool IsUsernameSecret => Username.StartsWith("secret:", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Determines if the password is a secret reference.
    /// </summary>
    public bool IsPasswordSecret => Password.StartsWith("secret:", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the secret name from a secret reference.
    /// </summary>
    /// <param name="secretReference">The secret reference (e.g., "secret:ftp-password").</param>
    /// <returns>The secret name (e.g., "ftp-password").</returns>
    public static string GetSecretName(string secretReference)
    {
        if (!secretReference.StartsWith("secret:", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid secret reference format. Expected 'secret:secretname'", nameof(secretReference));
        }

        return secretReference.Substring(7); // Remove "secret:" prefix
    }

    /// <summary>
    /// Validates the configuration and throws exceptions for invalid settings.
    /// </summary>
    public void Validate()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Host, nameof(Host));
        ArgumentException.ThrowIfNullOrWhiteSpace(Username, nameof(Username));
        ArgumentException.ThrowIfNullOrWhiteSpace(Password, nameof(Password));

        if (Port <= 0 || Port > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(Port), "Port must be between 1 and 65535");
        }

        if (TimeoutMilliseconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(TimeoutMilliseconds), "Timeout must be greater than 0");
        }

        if (BufferSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(BufferSize), "Buffer size must be greater than 0");
        }

        if (CredentialCacheDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(CredentialCacheDuration), "Cache duration must be greater than zero");
        }
    }
}