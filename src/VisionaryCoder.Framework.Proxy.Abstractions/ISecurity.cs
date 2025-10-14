// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace VisionaryCoder.Framework.Proxy.Abstractions;

/// <summary>
/// Defines a contract for security enrichers.
/// </summary>
public interface ISecurityEnricher
{
    /// <summary>
    /// Enriches the proxy context with security information.
    /// </summary>
    /// <param name="context">The proxy context to enrich.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task EnrichAsync(ProxyContext context);
}

/// <summary>
/// Defines a contract for authorization policies.
/// </summary>
public interface IAuthorizationPolicy
{
    /// <summary>
    /// Determines whether the operation is authorized.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>A task representing the asynchronous operation with the authorization result.</returns>
    Task<bool> IsAuthorizedAsync(ProxyContext context);
}

/// <summary>
/// Defines a contract for JWT token services.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Validates a JWT token.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>A task representing the asynchronous operation with the validation result.</returns>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// Gets claims from a JWT token.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <returns>A dictionary of claims.</returns>
    Task<Dictionary<string, object>> GetClaimsAsync(string token);
}