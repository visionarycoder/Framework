namespace VisionaryCoder.Framework.Azure.KeyVault;

/// <summary>
/// Configuration options for Azure Key Vault secret management.
/// </summary>
public sealed class KeyVaultOptions
{
    /// <summary>
    /// The URI of the Azure Key Vault instance.
    /// </summary>
    /// <example>https://your-keyvault.vault.azure.net/</example>
    public Uri? VaultUri { get; set; }

    /// <summary>
    /// The time-to-live for cached secrets.
    /// </summary>
    public TimeSpan CacheTtl { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Whether to use local secrets instead of Key Vault (for development).
    /// </summary>
    public bool UseLocalSecrets { get; set; } = false;

    /// <summary>
    /// The prefix to use when looking up local secrets in configuration.
    /// </summary>
    public string LocalSecretsPrefix { get; set; } = "Secrets";

    /// <summary>
    /// Maximum number of retry attempts for Key Vault operations.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// The delay between retry attempts.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
}