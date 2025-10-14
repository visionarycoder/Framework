// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Abstractions.Exceptions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Logging;

/// <summary>
/// Interceptor that logs proxy operations for monitoring and debugging purposes.
/// </summary>
public sealed class LoggingInterceptor(ILogger<LoggingInterceptor> logger) : IProxyInterceptor
{
    /// <summary>
    /// Invokes the interceptor with comprehensive logging of the proxy operation.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation with the response.</returns>
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next)
    {
        var operationName = context.OperationName ?? "Unknown";
        var correlationId = context.CorrelationId ?? "None";
        
        logger.LogDebug("Starting proxy operation '{OperationName}' with correlation ID '{CorrelationId}'", 
            operationName, correlationId);

        try
        {
            var response = await next(context);
            
            if (response.IsSuccess)
            {
                logger.LogInformation("Proxy operation '{OperationName}' completed successfully. Correlation ID: '{CorrelationId}'", 
                    operationName, correlationId);
            }
            else
            {
                logger.LogWarning("Proxy operation '{OperationName}' completed with failure. Error: '{ErrorMessage}'. Correlation ID: '{CorrelationId}'", 
                    operationName, response.ErrorMessage, correlationId);
            }

            return response;
        }
        catch (ProxyException ex)
        {
            logger.LogError(ex, "Proxy operation '{OperationName}' failed with proxy exception. Correlation ID: '{CorrelationId}'", 
                operationName, correlationId);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Proxy operation '{OperationName}' failed with unexpected exception. Correlation ID: '{CorrelationId}'", 
                operationName, correlationId);
            throw;
        }
    }
}

