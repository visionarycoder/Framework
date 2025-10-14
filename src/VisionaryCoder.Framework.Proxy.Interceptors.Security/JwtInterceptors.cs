using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Secrets.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// JWT interceptor specialized for Key Vault authentication scenarios.
/// Retrieves JWT tokens from Azure Key Vault and adds them to request headers.
/// </summary>
public class KeyVaultJwtInterceptor : IProxyInterceptor
{
    private readonly ISecretProvider secretProvider;
    private readonly ILogger<KeyVaultJwtInterceptor> logger;
    private readonly string secretName;
    private readonly string headerName;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyVaultJwtInterceptor"/> class.
    /// </summary>
    /// <param name="secretProvider">The secret provider for retrieving JWT tokens.</param>
    /// <param name="logger">The logger for diagnostic information.</param>
    /// <param name="secretName">The name of the secret in Key Vault containing the JWT token.</param>
    /// <param name="headerName">The header name to add the JWT token to. Defaults to "Authorization".</param>
    public KeyVaultJwtInterceptor(
        ISecretProvider secretProvider,
        ILogger<KeyVaultJwtInterceptor> logger,
        string secretName,
        string headerName = "Authorization")
    {
        this.secretProvider = secretProvider ?? throw new ArgumentNullException(nameof(secretProvider));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.secretName = secretName ?? throw new ArgumentNullException(nameof(secretName));
        this.headerName = headerName ?? throw new ArgumentNullException(nameof(headerName));
    }

    /// <summary>
    /// Intercepts the request and adds JWT authentication from Key Vault.
    /// </summary>
    /// <typeparam name="T">The response type.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        try
        {
            logger.LogDebug("Retrieving JWT token from Key Vault for secret: {SecretName}", secretName);
            
            var jwtToken = await secretProvider.GetAsync(secretName);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                // Ensure the token has the Bearer prefix if it's for Authorization header
                var tokenValue = headerName.Equals("Authorization", StringComparison.OrdinalIgnoreCase) && 
                                !jwtToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? $"Bearer {jwtToken}"
                    : jwtToken;

                context.Headers[headerName] = tokenValue;
                logger.LogDebug("JWT token added to {HeaderName} header", headerName);
            }
            else
            {
                logger.LogWarning("JWT token not found or empty for secret: {SecretName}", secretName);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve JWT token from Key Vault for secret: {SecretName}", secretName);
            // Continue without authentication - let downstream decide how to handle
        }

        return await next(context);
    }
}

/// <summary>
/// JWT interceptor for web-based authentication scenarios.
/// Handles OAuth flows and web-specific JWT token management.
/// </summary>
public class WebJwtInterceptor : IProxyInterceptor
{
    private readonly ITokenProvider tokenProvider;
    private readonly ILogger<WebJwtInterceptor> logger;
    private readonly WebJwtOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebJwtInterceptor"/> class.
    /// </summary>
    /// <param name="tokenProvider">The token provider for web authentication.</param>
    /// <param name="logger">The logger for diagnostic information.</param>
    /// <param name="options">The configuration options for web JWT handling.</param>
    public WebJwtInterceptor(
        ITokenProvider tokenProvider,
        ILogger<WebJwtInterceptor> logger,
        WebJwtOptions options)
    {
        this.tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Intercepts the request and adds web-based JWT authentication.
    /// </summary>
    /// <typeparam name="T">The response type.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        try
        {
            logger.LogDebug("Retrieving JWT token for audience: {Audience}", options.Audience);
            
            var tokenResult = await tokenProvider.GetTokenAsync(new TokenRequest
            {
                Audience = options.Audience,
                Scopes = options.Scopes,
                RefreshIfExpired = options.RefreshIfExpired
            });

            if (tokenResult.IsSuccess && !string.IsNullOrEmpty(tokenResult.AccessToken))
            {
                context.Headers[options.HeaderName] = $"Bearer {tokenResult.AccessToken}";
                logger.LogDebug("JWT token added to {HeaderName} header", options.HeaderName);
                
                // Add correlation ID if available
                if (!string.IsNullOrEmpty(tokenResult.CorrelationId))
                {
                    context.Headers["X-Correlation-ID"] = tokenResult.CorrelationId;
                }
            }
            else
            {
                logger.LogWarning("Failed to obtain JWT token: {Error}", tokenResult.Error);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve web JWT token for audience: {Audience}", options.Audience);
        }

        return await next(context);
    }
}

/// <summary>
/// Configuration options for web JWT authentication.
/// </summary>
public class WebJwtOptions
{
    /// <summary>
    /// Gets or sets the audience for the JWT token.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the scopes to request with the token.
    /// </summary>
    public ICollection<string> Scopes { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the header name to add the token to.
    /// </summary>
    public string HeaderName { get; set; } = "Authorization";

    /// <summary>
    /// Gets or sets a value indicating whether to refresh the token if expired.
    /// </summary>
    public bool RefreshIfExpired { get; set; } = true;
}

/// <summary>
/// Represents a token request for web authentication.
/// </summary>
public class TokenRequest
{
    /// <summary>
    /// Gets or sets the audience for the token.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the scopes to request.
    /// </summary>
    public ICollection<string> Scopes { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets a value indicating whether to refresh if expired.
    /// </summary>
    public bool RefreshIfExpired { get; set; } = true;
}

/// <summary>
/// Represents the result of a token request.
/// </summary>
public class TokenResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the request was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message if the request failed.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the correlation ID for the request.
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token expiration time.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }
}

/// <summary>
/// Interface for token providers in web scenarios.
/// </summary>
public interface ITokenProvider
{
    /// <summary>
    /// Gets a token asynchronously.
    /// </summary>
    /// <param name="request">The token request.</param>
    /// <returns>A task representing the token result.</returns>
    Task<TokenResult> GetTokenAsync(TokenRequest request);
}

