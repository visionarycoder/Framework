// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Helper class for enriching proxy context with JWT Bearer authentication.
/// </summary>
public sealed class JwtBearerEnricher : IProxySecurityEnricher
{
    private readonly ILogger<JwtBearerEnricher> _logger;
    private readonly Func<Task<string?>> _tokenProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtBearerEnricher"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="tokenProvider">Function that provides the JWT token.</param>
    public JwtBearerEnricher(ILogger<JwtBearerEnricher> logger, Func<Task<string?>> tokenProvider)
    {
        _logger = logger;
        _tokenProvider = tokenProvider;
    }

    /// <inheritdoc />
    public async Task EnrichAsync(ProxyContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await _tokenProvider();
            if (!string.IsNullOrEmpty(token))
            {
                context.Items["Authorization"] = $"Bearer {token}";
                _logger.LogDebug("JWT Bearer token added to context");
            }
            else
            {
                _logger.LogWarning("JWT token provider returned null or empty token");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve JWT token");
            throw;
        }
    }
}