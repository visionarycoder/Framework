using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Abstractions;
using VisionaryCoder.Framework.Services.Abstractions;
using VisionaryCoder.Framework.Secrets.Abstractions;

namespace VisionaryCoder.Framework.Services.FileSystem;

/// <summary>
/// Secure FTP-based file system operations implementation that integrates with ISecretProvider.
/// This service retrieves FTP credentials from secure secret stores (like Azure Key Vault)
/// and provides the same IFileSystem interface with enhanced security.
/// </summary>
public sealed class SecureFtpFileSystemService : ServiceBase<SecureFtpFileSystemService>, IFileSystem
{
    private readonly SecureFtpFileSystemOptions _options;
    private readonly ISecretProvider _secretProvider;
    private readonly IMemoryCache _credentialCache;
    private readonly string _credentialCacheKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecureFtpFileSystemService"/> class.
    /// </summary>
    /// <param name="options">The secure FTP configuration options.</param>
    /// <param name="secretProvider">The secret provider for retrieving credentials.</param>
    /// <param name="cache">The memory cache for credential caching.</param>
    /// <param name="logger">The logger instance for this service.</param>
    public SecureFtpFileSystemService(
        SecureFtpFileSystemOptions options,
        ISecretProvider secretProvider,
        IMemoryCache cache,
        ILogger<SecureFtpFileSystemService> logger) 
        : base(logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _secretProvider = secretProvider ?? throw new ArgumentNullException(nameof(secretProvider));
        _credentialCache = cache ?? throw new ArgumentNullException(nameof(cache));
        
        // Validate configuration
        _options.Validate();
        
        _credentialCacheKey = $"ftp-credentials:{_options.Host}:{_options.Username}";
        
        Logger.LogDebug("Initialized SecureFtpFileSystemService for host {Host}", _options.Host);
    }

    #region Credential Management

    /// <summary>
    /// Retrieves FTP credentials, resolving secrets as needed and caching for performance.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A NetworkCredential object with resolved username and password.</returns>
    private async Task<NetworkCredential> GetCredentialsAsync(CancellationToken cancellationToken = default)
    {
        // Check cache first if caching is enabled
        if (_options.CacheCredentials && _credentialCache.TryGetValue(_credentialCacheKey, out NetworkCredential? cachedCredentials))
        {
            Logger.LogTrace("Retrieved cached FTP credentials for {Host}", _options.Host);
            return cachedCredentials;
        }

        try
        {
            Logger.LogDebug("Resolving FTP credentials for {Host}", _options.Host);

            // Resolve username (may be a secret reference)
            var username = await ResolveCredentialValueAsync(_options.Username, "username", cancellationToken);
            
            // Resolve password (may be a secret reference)
            var password = await ResolveCredentialValueAsync(_options.Password, "password", cancellationToken);

            var credentials = new NetworkCredential(username, password);

            // Cache credentials if enabled
            if (_options.CacheCredentials)
            {
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _options.CredentialCacheDuration,
                    Priority = CacheItemPriority.Normal
                };

                _credentialCache.Set(_credentialCacheKey, credentials, cacheOptions);
                Logger.LogTrace("Cached FTP credentials for {Host} (expires in {Duration})", _options.Host, _options.CredentialCacheDuration);
            }

            Logger.LogDebug("Successfully resolved FTP credentials for {Host}", _options.Host);
            return credentials;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to resolve FTP credentials for {Host}", _options.Host);
            throw;
        }
    }

    /// <summary>
    /// Resolves a credential value, handling both direct values and secret references.
    /// </summary>
    /// <param name="value">The value or secret reference.</param>
    /// <param name="credentialType">The type of credential (for logging).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The resolved credential value.</returns>
    private async Task<string> ResolveCredentialValueAsync(string value, string credentialType, CancellationToken cancellationToken)
    {
        if (!value.StartsWith("secret:", StringComparison.OrdinalIgnoreCase))
        {
            // Direct value, return as-is (but don't log it for security)
            Logger.LogTrace("Using direct {CredentialType} value for {Host}", credentialType, _options.Host);
            return value;
        }

        // Secret reference, resolve from secret provider
        var secretName = SecureFtpFileSystemOptions.GetSecretName(value);
        Logger.LogDebug("Resolving {CredentialType} secret '{SecretName}' for {Host}", credentialType, secretName, _options.Host);

        var secretValue = await _secretProvider.GetAsync(secretName, cancellationToken);
        
        if (string.IsNullOrEmpty(secretValue))
        {
            throw new InvalidOperationException($"Secret '{secretName}' for FTP {credentialType} not found or is empty");
        }

        Logger.LogTrace("Successfully resolved {CredentialType} secret for {Host}", credentialType, _options.Host);
        return secretValue;
    }

    /// <summary>
    /// Clears cached credentials, forcing fresh retrieval on next access.
    /// </summary>
    public void ClearCredentialCache()
    {
        _credentialCache.Remove(_credentialCacheKey);
        Logger.LogDebug("Cleared cached FTP credentials for {Host}", _options.Host);
    }

    #endregion

    #region FTP Request Creation

    /// <summary>
    /// Creates and configures an FtpWebRequest for the specified path and method with secure credentials.
    /// </summary>
    /// <param name="path">The FTP path for the request.</param>
    /// <param name="method">The FTP method to use.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A configured FtpWebRequest with secure credentials.</returns>
    private async Task<FtpWebRequest> CreateSecureFtpWebRequestAsync(string path, string method, CancellationToken cancellationToken = default)
    {
        var normalizedPath = path.Replace('\\', '/');
        if (!normalizedPath.StartsWith('/'))
        {
            normalizedPath = '/' + normalizedPath;
        }

        var uri = $"{_options.ServerUri.TrimEnd('/')}{normalizedPath}";
        var request = (FtpWebRequest)WebRequest.Create(uri);
        
        request.Method = method;
        request.UsePassive = _options.UsePassive;
        request.UseBinary = _options.UseBinary;
        request.KeepAlive = _options.KeepAlive;
        request.Timeout = _options.TimeoutMilliseconds;
        
        if (_options.UseSsl)
        {
            request.EnableSsl = true;
        }

        // Get secure credentials
        var credentials = await GetCredentialsAsync(cancellationToken);
        request.Credentials = credentials;

        Logger.LogTrace("Created secure FTP request: {Method} {Uri}", method, uri);
        return request;
    }

    #endregion

    #region File Operations

    /// <inheritdoc />
    public bool FileExists(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        return FileExistsAsync(path).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Async version of FileExists for better performance with credential resolution.
    /// </summary>
    public async Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        try
        {
            Logger.LogDebug("Checking secure FTP file existence for '{Path}' on {Host}", path, _options.Host);
            
            var request = await CreateSecureFtpWebRequestAsync(path, WebRequestMethods.Ftp.GetFileSize, cancellationToken);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            
            var exists = response.StatusCode == FtpStatusCode.FileStatus;
            Logger.LogTrace("Secure FTP file existence check for '{Path}' on {Host}: {Exists}", path, _options.Host, exists);
            return exists;
        }
        catch (WebException ex) when (ex.Response is FtpWebResponse ftpResponse && 
                                     ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
        {
            Logger.LogTrace("Secure FTP file '{Path}' does not exist on {Host}", path, _options.Host);
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking secure FTP file existence for '{Path}' on {Host}", path, _options.Host);
            throw;
        }
    }

    /// <inheritdoc />
    public bool FileExists(FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        return FileExists(fileInfo.FullName);
    }

    /// <inheritdoc />
    public string ReadAllText(string path)
    {
        return ReadAllTextAsync(path).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Reading all text from secure FTP file '{Path}' on {Host}", path, _options.Host);
            
            var request = await CreateSecureFtpWebRequestAsync(path, WebRequestMethods.Ftp.DownloadFile, cancellationToken);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            
            var content = await reader.ReadToEndAsync(cancellationToken);
            Logger.LogTrace("Successfully read {Length} characters from secure FTP file '{Path}' on {Host}", content.Length, path, _options.Host);
            return content;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reading text from secure FTP file '{Path}' on {Host}", path, _options.Host);
            throw;
        }
    }

    /// <inheritdoc />
    public byte[] ReadAllBytes(string path)
    {
        return ReadAllBytesAsync(path).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Reading all bytes from secure FTP file '{Path}' on {Host}", path, _options.Host);
            
            var request = await CreateSecureFtpWebRequestAsync(path, WebRequestMethods.Ftp.DownloadFile, cancellationToken);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            using var stream = response.GetResponseStream();
            using var memoryStream = new MemoryStream();
            
            await stream.CopyToAsync(memoryStream, _options.BufferSize, cancellationToken);
            var bytes = memoryStream.ToArray();
            
            Logger.LogTrace("Successfully read {Length} bytes from secure FTP file '{Path}' on {Host}", bytes.Length, path, _options.Host);
            return bytes;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reading bytes from secure FTP file '{Path}' on {Host}", path, _options.Host);
            throw;
        }
    }

    /// <inheritdoc />
    public void WriteAllText(string path, string content)
    {
        WriteAllTextAsync(path, content).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public async Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(content);

        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        await WriteAllBytesAsync(path, bytes, cancellationToken);
    }

    /// <inheritdoc />
    public void WriteAllBytes(string path, byte[] bytes)
    {
        WriteAllBytesAsync(path, bytes).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public async Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(bytes);

        try
        {
            Logger.LogDebug("Writing {Length} bytes to secure FTP file '{Path}' on {Host}", bytes.Length, path, _options.Host);
            
            var request = await CreateSecureFtpWebRequestAsync(path, WebRequestMethods.Ftp.UploadFile, cancellationToken);
            request.ContentLength = bytes.Length;
            
            using var stream = await request.GetRequestStreamAsync();
            await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
            
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            Logger.LogTrace("Successfully wrote {Length} bytes to secure FTP file '{Path}' on {Host} (Status: {Status})", 
                           bytes.Length, path, _options.Host, response.StatusCode);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error writing bytes to secure FTP file '{Path}' on {Host}", path, _options.Host);
            throw;
        }
    }

    /// <inheritdoc />
    public void DeleteFile(string path)
    {
        DeleteFileAsync(path).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public async Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            if (!(await FileExistsAsync(path, cancellationToken)))
            {
                Logger.LogTrace("Secure FTP file '{Path}' does not exist on {Host}, no deletion needed", path, _options.Host);
                return;
            }

            Logger.LogDebug("Deleting secure FTP file '{Path}' on {Host}", path, _options.Host);
            
            var request = await CreateSecureFtpWebRequestAsync(path, WebRequestMethods.Ftp.DeleteFile, cancellationToken);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            
            Logger.LogTrace("Successfully deleted secure FTP file '{Path}' on {Host} (Status: {Status})", path, _options.Host, response.StatusCode);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting secure FTP file '{Path}' on {Host}", path, _options.Host);
            throw;
        }
    }

    #endregion

    #region Directory Operations (Simplified for brevity - same pattern as file operations)

    /// <inheritdoc />
    public bool DirectoryExists(string path)
    {
        return DirectoryExistsAsync(path).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Async version of DirectoryExists for better performance with credential resolution.
    /// </summary>
    public async Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        try
        {
            Logger.LogDebug("Checking secure FTP directory existence for '{Path}' on {Host}", path, _options.Host);
            
            var request = await CreateSecureFtpWebRequestAsync(path, WebRequestMethods.Ftp.ListDirectory, cancellationToken);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            
            var exists = response.StatusCode == FtpStatusCode.DataAlreadyOpen || 
                        response.StatusCode == FtpStatusCode.OpeningData;
            Logger.LogTrace("Secure FTP directory existence check for '{Path}' on {Host}: {Exists}", path, _options.Host, exists);
            return exists;
        }
        catch (WebException ex) when (ex.Response is FtpWebResponse ftpResponse && 
                                     ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
        {
            Logger.LogTrace("Secure FTP directory '{Path}' does not exist on {Host}", path, _options.Host);
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking secure FTP directory existence for '{Path}' on {Host}", path, _options.Host);
            throw;
        }
    }

    /// <inheritdoc />
    public DirectoryInfo CreateDirectory(string path)
    {
        return CreateDirectoryAsync(path).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public async Task<DirectoryInfo> CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Creating secure FTP directory '{Path}' on {Host}", path, _options.Host);
            
            var request = await CreateSecureFtpWebRequestAsync(path, WebRequestMethods.Ftp.MakeDirectory, cancellationToken);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            
            Logger.LogTrace("Successfully created secure FTP directory '{Path}' on {Host} (Status: {Status})", path, _options.Host, response.StatusCode);
            return new DirectoryInfo(path);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating secure FTP directory '{Path}' on {Host}", path, _options.Host);
            throw;
        }
    }

    /// <inheritdoc />
    public void DeleteDirectory(string path, bool recursive = true)
    {
        DeleteDirectoryAsync(path, recursive).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public async Task DeleteDirectoryAsync(string path, bool recursive = true, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            if (!(await DirectoryExistsAsync(path, cancellationToken)))
            {
                Logger.LogTrace("Secure FTP directory '{Path}' does not exist on {Host}, no deletion needed", path, _options.Host);
                return;
            }

            Logger.LogDebug("Deleting secure FTP directory '{Path}' on {Host} (recursive: {Recursive})", path, _options.Host, recursive);

            if (recursive)
            {
                // Delete all files and subdirectories first
                var files = await GetFilesAsync(path, "*", cancellationToken);
                foreach (var file in files)
                {
                    await DeleteFileAsync(file, cancellationToken);
                }

                var directories = await GetDirectoriesAsync(path, "*", cancellationToken);
                foreach (var directory in directories)
                {
                    await DeleteDirectoryAsync(directory, true, cancellationToken);
                }
            }
            
            var request = await CreateSecureFtpWebRequestAsync(path, WebRequestMethods.Ftp.RemoveDirectory, cancellationToken);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            
            Logger.LogTrace("Successfully deleted secure FTP directory '{Path}' on {Host} (Status: {Status})", path, _options.Host, response.StatusCode);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting secure FTP directory '{Path}' on {Host} (recursive: {Recursive})", path, _options.Host, recursive);
            throw;
        }
    }

    /// <inheritdoc />
    public string[] GetFiles(string path, string searchPattern = "*")
    {
        return GetFilesAsync(path, searchPattern).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Async version of GetFiles for better performance with credential resolution.
    /// </summary>
    public async Task<string[]> GetFilesAsync(string path, string searchPattern = "*", CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(searchPattern);

        try
        {
            Logger.LogDebug("Getting secure FTP files from '{Path}' on {Host} with pattern '{Pattern}'", path, _options.Host, searchPattern);
            
            var request = await CreateSecureFtpWebRequestAsync(path, WebRequestMethods.Ftp.ListDirectory, cancellationToken);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            
            var files = new List<string>();
            string? line;
            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                // Simple pattern matching (FTP doesn't support server-side filtering)
                if (MatchesPattern(line, searchPattern))
                {
                    files.Add(CombinePath(path, line));
                }
            }
            
            Logger.LogTrace("Found {Count} secure FTP files in '{Path}' on {Host}", files.Count, path, _options.Host);
            return files.ToArray();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting secure FTP files from '{Path}' on {Host} with pattern '{Pattern}'", path, _options.Host, searchPattern);
            throw;
        }
    }

    /// <inheritdoc />
    public string[] GetDirectories(string path, string searchPattern = "*")
    {
        return GetDirectoriesAsync(path, searchPattern).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Async version of GetDirectories for better performance with credential resolution.
    /// </summary>
    public async Task<string[]> GetDirectoriesAsync(string path, string searchPattern = "*", CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(searchPattern);

        try
        {
            Logger.LogDebug("Getting secure FTP directories from '{Path}' on {Host} with pattern '{Pattern}'", path, _options.Host, searchPattern);
            
            var request = await CreateSecureFtpWebRequestAsync(path, WebRequestMethods.Ftp.ListDirectoryDetails, cancellationToken);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            
            var directories = new List<string>();
            string? line;
            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                // Parse directory listing (simplified version)
                if (line.StartsWith('d') && MatchesPattern(ExtractFileName(line), searchPattern))
                {
                    directories.Add(CombinePath(path, ExtractFileName(line)));
                }
            }
            
            Logger.LogTrace("Found {Count} secure FTP directories in '{Path}' on {Host}", directories.Count, path, _options.Host);
            return directories.ToArray();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting secure FTP directories from '{Path}' on {Host} with pattern '{Pattern}'", path, _options.Host, searchPattern);
            throw;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern = "*", [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(searchPattern);

        Logger.LogDebug("Enumerating secure FTP files async from '{Path}' on {Host} with pattern '{Pattern}'", path, _options.Host, searchPattern);

        var files = await GetFilesAsync(path, searchPattern, cancellationToken);
        
        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return file;
        }

        Logger.LogTrace("Completed enumerating secure FTP files from '{Path}' on {Host}", path, _options.Host);
    }

    #endregion

    #region Path Utilities

    /// <inheritdoc />
    public string GetFullPath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            // For FTP, we construct the full URI path
            var fullPath = path.StartsWith('/') ? path : $"/{path.TrimStart('/')}";
            Logger.LogTrace("Resolved secure FTP full path for '{Path}': '{FullPath}'", path, fullPath);
            return fullPath;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resolving secure FTP full path for '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public string? GetDirectoryName(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            var directoryName = Path.GetDirectoryName(path)?.Replace('\\', '/');
            Logger.LogTrace("Resolved secure FTP directory name for '{Path}': '{DirectoryName}'", path, directoryName ?? "<null>");
            return directoryName;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resolving secure FTP directory name for '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public string GetFileName(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            var fileName = Path.GetFileName(path);
            Logger.LogTrace("Resolved secure FTP file name for '{Path}': '{FileName}'", path, fileName);
            return fileName;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resolving secure FTP file name for '{Path}'", path);
            throw;
        }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Combines FTP path segments properly.
    /// </summary>
    /// <param name="path1">The base path.</param>
    /// <param name="path2">The path to combine.</param>
    /// <returns>The combined FTP path.</returns>
    private static string CombinePath(string path1, string path2)
    {
        var normalizedPath1 = path1.Replace('\\', '/').TrimEnd('/');
        var normalizedPath2 = path2.Replace('\\', '/').TrimStart('/');
        return $"{normalizedPath1}/{normalizedPath2}";
    }

    /// <summary>
    /// Checks if a filename matches the specified pattern.
    /// </summary>
    /// <param name="fileName">The filename to check.</param>
    /// <param name="pattern">The pattern to match against.</param>
    /// <returns>True if the filename matches the pattern.</returns>
    private static bool MatchesPattern(string fileName, string pattern)
    {
        if (pattern == "*")
            return true;

        // Simple wildcard matching (can be enhanced for more complex patterns)
        var regexPattern = pattern.Replace("*", ".*").Replace("?", ".");
        return System.Text.RegularExpressions.Regex.IsMatch(fileName, $"^{regexPattern}$", 
               System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Extracts the filename from an FTP directory listing line.
    /// </summary>
    /// <param name="listingLine">The FTP directory listing line.</param>
    /// <returns>The extracted filename.</returns>
    private static string ExtractFileName(string listingLine)
    {
        // This is a simplified parser for FTP directory listings
        // In production, you might want to use a more robust FTP listing parser
        var parts = listingLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[^1] : listingLine;
    }

    #endregion
}