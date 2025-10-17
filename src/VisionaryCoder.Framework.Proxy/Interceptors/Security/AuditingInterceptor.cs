using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Proxy.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Interceptors.Security;

/// <summary>
/// Interceptor that provides comprehensive auditing of proxy requests and responses.
/// </summary>
[ProxyInterceptorOrder(100)] // Execute early to capture all activity
public class AuditingInterceptor : IProxyInterceptor
{
    private readonly IAuditSink auditSink;
    private readonly ILogger<AuditingInterceptor> logger;
    private readonly AuditingOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditingInterceptor"/> class.
    /// </summary>
    /// <param name="auditSink">The sink for persisting audit records.</param>
    /// <param name="logger">The logger for diagnostic information.</param>
    /// <param name="options">The auditing configuration options.</param>
    public AuditingInterceptor(
        IAuditSink auditSink,
        ILogger<AuditingInterceptor> logger,
        AuditingOptions options)
    {
        this.auditSink = auditSink ?? throw new ArgumentNullException(nameof(auditSink));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Intercepts the request to provide auditing capabilities.
    /// </summary>
    /// <typeparam name="T">The response type.</typeparam>
    /// <param name="context">The proxy context.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<Response<T>> InvokeAsync<T>(ProxyContext context, ProxyDelegate<T> next, CancellationToken cancellationToken = default)
    {
        var auditRecord = CreateAuditRecord(context);
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            logger.LogDebug("Starting audit for request: {RequestId}", auditRecord.RequestId);
            
            var response = await next(context, cancellationToken);
            
            stopwatch.Stop();
            auditRecord.CompletedAt = DateTimeOffset.UtcNow;
            auditRecord.Duration = stopwatch.Elapsed;
            auditRecord.StatusCode = response.StatusCode;
            auditRecord.IsSuccess = response.IsSuccess;
            
            if (!response.IsSuccess && options.IncludeErrorDetails)
            {
                auditRecord.ErrorMessage = response.ErrorMessage;
            }
            
            if (options.IncludeResponseData && response.IsSuccess && response.Data != null)
            {
                auditRecord.ResponseSize = CalculateResponseSize(response.Data);
            }
            
            await auditSink.WriteAsync(auditRecord, cancellationToken);
            
            logger.LogDebug("Completed audit for request: {RequestId}, Duration: {Duration}ms", 
                auditRecord.RequestId, auditRecord.Duration?.TotalMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            auditRecord.CompletedAt = DateTimeOffset.UtcNow;
            auditRecord.Duration = stopwatch.Elapsed;
            auditRecord.IsSuccess = false;
            auditRecord.ErrorMessage = ex.Message;
            auditRecord.ExceptionType = ex.GetType().Name;
            
            try
            {
                await auditSink.WriteAsync(auditRecord, cancellationToken);
            }
            catch (Exception auditEx)
            {
                logger.LogError(auditEx, "Failed to write audit record for request: {RequestId}", auditRecord.RequestId);
            }
            
            logger.LogError(ex, "Request failed during audit for: {RequestId}", auditRecord.RequestId);
            throw;
        }
    }

    /// <summary>
    /// Creates an audit record from the proxy context.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>An audit record.</returns>
    private AuditRecord CreateAuditRecord(ProxyContext context)
    {
        return new AuditRecord
        {
            RequestId = context.RequestId,
            UserId = ExtractUserId(context),
            UserAgent = ExtractUserAgent(context),
            IpAddress = ExtractIpAddress(context),
            Method = context.Method,
            Url = context.Url,
            StartedAt = DateTimeOffset.UtcNow,
            Headers = options.IncludeHeaders ? SanitizeHeaders(context.Headers) : null,
            RequestSize = CalculateRequestSize(context)
        };
    }

    /// <summary>
    /// Extracts the user ID from the context.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>The user ID if available.</returns>
    private static string? ExtractUserId(ProxyContext context)
    {
        // Look for common user identification patterns
        if (context.Headers.TryGetValue("X-User-ID", out var userId))
            return userId;
        
        if (context.Headers.TryGetValue("Authorization", out var auth) && auth.StartsWith("Bearer "))
        {
            // Could extract from JWT token here if needed
            return "jwt-user";
        }
        
        return null;
    }

    /// <summary>
    /// Extracts the user agent from the context.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>The user agent if available.</returns>
    private static string? ExtractUserAgent(ProxyContext context)
    {
        context.Headers.TryGetValue("User-Agent", out var userAgent);
        return userAgent;
    }

    /// <summary>
    /// Extracts the IP address from the context.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>The IP address if available.</returns>
    private static string? ExtractIpAddress(ProxyContext context)
    {
        // Look for forwarded IP headers first
        if (context.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var firstIp = forwardedFor.Split(',').FirstOrDefault()?.Trim();
            if (!string.IsNullOrEmpty(firstIp))
                return firstIp;
        }
        
        if (context.Headers.TryGetValue("X-Real-IP", out var realIp))
            return realIp;
        
        if (context.Headers.TryGetValue("Remote-Addr", out var remoteAddr))
            return remoteAddr;
        
        return null;
    }

    /// <summary>
    /// Sanitizes headers to remove sensitive information.
    /// </summary>
    /// <param name="headers">The headers to sanitize.</param>
    /// <returns>Sanitized headers.</returns>
    private Dictionary<string, string>? SanitizeHeaders(IDictionary<string, string> headers)
    {
        if (headers == null || headers.Count == 0)
            return null;

        var sanitized = new Dictionary<string, string>();
        
        foreach (var header in headers)
        {
            var key = header.Key;
            var value = header.Value;
            
            // Sanitize sensitive headers
            if (IsSensitiveHeader(key))
            {
                value = "***REDACTED***";
            }
            
            sanitized[key] = value;
        }
        
        return sanitized;
    }

    /// <summary>
    /// Determines if a header contains sensitive information.
    /// </summary>
    /// <param name="headerName">The header name.</param>
    /// <returns>True if sensitive.</returns>
    private static bool IsSensitiveHeader(string headerName)
    {
        var sensitiveHeaders = new[]
        {
            "Authorization",
            "Cookie",
            "Set-Cookie",
            "X-API-Key",
            "X-Auth-Token"
        };
        
        return sensitiveHeaders.Any(h => h.Equals(headerName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Calculates the request size.
    /// </summary>
    /// <param name="context">The proxy context.</param>
    /// <returns>The request size in bytes.</returns>
    private static long CalculateRequestSize(ProxyContext context)
    {
        // Basic calculation - could be enhanced based on actual request body
        var headerSize = context.Headers.Sum(h => h.Key.Length + h.Value.Length);
        var urlSize = context.Url?.Length ?? 0;
        
        return headerSize + urlSize;
    }

    /// <summary>
    /// Calculates the response size.
    /// </summary>
    /// <param name="data">The response data.</param>
    /// <returns>The response size in bytes.</returns>
    private static long CalculateResponseSize(object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            return System.Text.Encoding.UTF8.GetByteCount(json);
        }
        catch
        {
            // If serialization fails, return an estimate
            return data.ToString()?.Length ?? 0;
        }
    }
}