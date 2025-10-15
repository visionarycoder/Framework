using System.Net;
using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Abstractions;
using VisionaryCoder.Framework.Services.Abstractions;

namespace VisionaryCoder.Framework.Services.FileSystem;

/// <summary>
/// Configuration options for FTP file system operations.
/// </summary>
public sealed class FtpFileSystemOptions
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
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Gets or sets the password for FTP authentication.
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
    /// Gets the FTP server URI based on the configuration.
    /// </summary>
    public string ServerUri => UseSsl ? $"ftps://{Host}:{Port}" : $"ftp://{Host}:{Port}";
}

/// <summary>
/// Provides FTP-based file system operations implementation following Microsoft I/O patterns.
/// This service wraps FTP operations with logging, error handling, and async support.
/// Supports both standard FTP and secure FTPS protocols.
/// </summary>
public sealed class FtpFileSystemService : ServiceBase<FtpFileSystemService>, IFileSystem
{
    private readonly FtpFileSystemOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="FtpFileSystemService"/> class.
    /// </summary>
    /// <param name="options">The FTP configuration options.</param>
    /// <param name="logger">The logger instance for this service.</param>
    public FtpFileSystemService(FtpFileSystemOptions options, ILogger<FtpFileSystemService> logger) 
        : base(logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        
        // Validate required options
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Host, nameof(options.Host));
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Username, nameof(options.Username));
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Password, nameof(options.Password));
    }

    #region File Operations

    /// <inheritdoc />
    public bool FileExists(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        try
        {
            Logger.LogDebug("Checking FTP file existence for '{Path}'", path);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.GetFileSize);
            using var response = (FtpWebResponse)request.GetResponse();
            
            var exists = response.StatusCode == FtpStatusCode.FileStatus;
            Logger.LogTrace("FTP file existence check for '{Path}': {Exists}", path, exists);
            return exists;
        }
        catch (WebException ex) when (ex.Response is FtpWebResponse ftpResponse && 
                                     ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
        {
            Logger.LogTrace("FTP file '{Path}' does not exist", path);
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking FTP file existence for '{Path}'", path);
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
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Reading all text from FTP file '{Path}'", path);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.DownloadFile);
            using var response = (FtpWebResponse)request.GetResponse();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            
            var content = reader.ReadToEnd();
            Logger.LogTrace("Successfully read {Length} characters from FTP file '{Path}'", content.Length, path);
            return content;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reading text from FTP file '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Reading all text async from FTP file '{Path}'", path);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.DownloadFile);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            
            var content = await reader.ReadToEndAsync(cancellationToken);
            Logger.LogTrace("Successfully read {Length} characters from FTP file '{Path}'", content.Length, path);
            return content;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reading text async from FTP file '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public byte[] ReadAllBytes(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Reading all bytes from FTP file '{Path}'", path);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.DownloadFile);
            using var response = (FtpWebResponse)request.GetResponse();
            using var stream = response.GetResponseStream();
            using var memoryStream = new MemoryStream();
            
            stream.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();
            
            Logger.LogTrace("Successfully read {Length} bytes from FTP file '{Path}'", bytes.Length, path);
            return bytes;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reading bytes from FTP file '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Reading all bytes async from FTP file '{Path}'", path);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.DownloadFile);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            using var stream = response.GetResponseStream();
            using var memoryStream = new MemoryStream();
            
            await stream.CopyToAsync(memoryStream, _options.BufferSize, cancellationToken);
            var bytes = memoryStream.ToArray();
            
            Logger.LogTrace("Successfully read {Length} bytes from FTP file '{Path}'", bytes.Length, path);
            return bytes;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reading bytes async from FTP file '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public void WriteAllText(string path, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(content);

        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        WriteAllBytes(path, bytes);
    }

    /// <inheritdoc />
    public Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(content);

        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        return WriteAllBytesAsync(path, bytes, cancellationToken);
    }

    /// <inheritdoc />
    public void WriteAllBytes(string path, byte[] bytes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(bytes);

        try
        {
            Logger.LogDebug("Writing {Length} bytes to FTP file '{Path}'", bytes.Length, path);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.UploadFile);
            request.ContentLength = bytes.Length;
            
            using var stream = request.GetRequestStream();
            stream.Write(bytes, 0, bytes.Length);
            
            using var response = (FtpWebResponse)request.GetResponse();
            Logger.LogTrace("Successfully wrote {Length} bytes to FTP file '{Path}' (Status: {Status})", 
                           bytes.Length, path, response.StatusCode);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error writing bytes to FTP file '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(bytes);

        try
        {
            Logger.LogDebug("Writing {Length} bytes async to FTP file '{Path}'", bytes.Length, path);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.UploadFile);
            request.ContentLength = bytes.Length;
            
            using var stream = await request.GetRequestStreamAsync();
            await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
            
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            Logger.LogTrace("Successfully wrote {Length} bytes async to FTP file '{Path}' (Status: {Status})", 
                           bytes.Length, path, response.StatusCode);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error writing bytes async to FTP file '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public void DeleteFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            if (!FileExists(path))
            {
                Logger.LogTrace("FTP file '{Path}' does not exist, no deletion needed", path);
                return;
            }

            Logger.LogDebug("Deleting FTP file '{Path}'", path);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.DeleteFile);
            using var response = (FtpWebResponse)request.GetResponse();
            
            Logger.LogTrace("Successfully deleted FTP file '{Path}' (Status: {Status})", path, response.StatusCode);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting FTP file '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            if (!FileExists(path))
            {
                Logger.LogTrace("FTP file '{Path}' does not exist, no deletion needed", path);
                return;
            }

            Logger.LogDebug("Deleting FTP file async '{Path}'", path);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.DeleteFile);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            
            Logger.LogTrace("Successfully deleted FTP file async '{Path}' (Status: {Status})", path, response.StatusCode);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting FTP file async '{Path}'", path);
            throw;
        }
    }

    #endregion

    #region Directory Operations

    /// <inheritdoc />
    public bool DirectoryExists(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        try
        {
            Logger.LogDebug("Checking FTP directory existence for '{Path}'", path);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.ListDirectory);
            using var response = (FtpWebResponse)request.GetResponse();
            
            var exists = response.StatusCode == FtpStatusCode.DataAlreadyOpen || 
                        response.StatusCode == FtpStatusCode.OpeningData;
            Logger.LogTrace("FTP directory existence check for '{Path}': {Exists}", path, exists);
            return exists;
        }
        catch (WebException ex) when (ex.Response is FtpWebResponse ftpResponse && 
                                     ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
        {
            Logger.LogTrace("FTP directory '{Path}' does not exist", path);
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking FTP directory existence for '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public DirectoryInfo CreateDirectory(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Creating FTP directory '{Path}'", path);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.MakeDirectory);
            using var response = (FtpWebResponse)request.GetResponse();
            
            Logger.LogTrace("Successfully created FTP directory '{Path}' (Status: {Status})", path, response.StatusCode);
            
            // Return a DirectoryInfo-like object (FTP doesn't provide local DirectoryInfo)
            return new DirectoryInfo(path);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating FTP directory '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<DirectoryInfo> CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Creating FTP directory async '{Path}'", path);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.MakeDirectory);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            
            Logger.LogTrace("Successfully created FTP directory async '{Path}' (Status: {Status})", path, response.StatusCode);
            
            return new DirectoryInfo(path);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating FTP directory async '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public void DeleteDirectory(string path, bool recursive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            if (!DirectoryExists(path))
            {
                Logger.LogTrace("FTP directory '{Path}' does not exist, no deletion needed", path);
                return;
            }

            Logger.LogDebug("Deleting FTP directory '{Path}' (recursive: {Recursive})", path, recursive);

            if (recursive)
            {
                // Delete all files and subdirectories first
                var files = GetFiles(path);
                foreach (var file in files)
                {
                    DeleteFile(file);
                }

                var directories = GetDirectories(path);
                foreach (var directory in directories)
                {
                    DeleteDirectory(directory, true);
                }
            }
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.RemoveDirectory);
            using var response = (FtpWebResponse)request.GetResponse();
            
            Logger.LogTrace("Successfully deleted FTP directory '{Path}' (Status: {Status})", path, response.StatusCode);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting FTP directory '{Path}' (recursive: {Recursive})", path, recursive);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteDirectoryAsync(string path, bool recursive = true, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            if (!DirectoryExists(path))
            {
                Logger.LogTrace("FTP directory '{Path}' does not exist, no deletion needed", path);
                return;
            }

            Logger.LogDebug("Deleting FTP directory async '{Path}' (recursive: {Recursive})", path, recursive);

            if (recursive)
            {
                // Delete all files and subdirectories first
                var files = GetFiles(path);
                foreach (var file in files)
                {
                    await DeleteFileAsync(file, cancellationToken);
                }

                var directories = GetDirectories(path);
                foreach (var directory in directories)
                {
                    await DeleteDirectoryAsync(directory, true, cancellationToken);
                }
            }
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.RemoveDirectory);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            
            Logger.LogTrace("Successfully deleted FTP directory async '{Path}' (Status: {Status})", path, response.StatusCode);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting FTP directory async '{Path}' (recursive: {Recursive})", path, recursive);
            throw;
        }
    }

    /// <inheritdoc />
    public string[] GetFiles(string path, string searchPattern = "*")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(searchPattern);

        try
        {
            Logger.LogDebug("Getting FTP files from '{Path}' with pattern '{Pattern}'", path, searchPattern);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.ListDirectory);
            using var response = (FtpWebResponse)request.GetResponse();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            
            var files = new List<string>();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                // Simple pattern matching (FTP doesn't support server-side filtering)
                if (MatchesPattern(line, searchPattern))
                {
                    files.Add(CombinePath(path, line));
                }
            }
            
            Logger.LogTrace("Found {Count} FTP files in '{Path}'", files.Count, path);
            return files.ToArray();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting FTP files from '{Path}' with pattern '{Pattern}'", path, searchPattern);
            throw;
        }
    }

    /// <inheritdoc />
    public string[] GetDirectories(string path, string searchPattern = "*")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(searchPattern);

        try
        {
            Logger.LogDebug("Getting FTP directories from '{Path}' with pattern '{Pattern}'", path, searchPattern);
            
            var request = CreateFtpWebRequest(path, WebRequestMethods.Ftp.ListDirectoryDetails);
            using var response = (FtpWebResponse)request.GetResponse();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            
            var directories = new List<string>();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                // Parse directory listing (this is a simplified version)
                if (line.StartsWith('d') && MatchesPattern(ExtractFileName(line), searchPattern))
                {
                    directories.Add(CombinePath(path, ExtractFileName(line)));
                }
            }
            
            Logger.LogTrace("Found {Count} FTP directories in '{Path}'", directories.Count, path);
            return directories.ToArray();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting FTP directories from '{Path}' with pattern '{Pattern}'", path, searchPattern);
            throw;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern = "*", [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(searchPattern);

        Logger.LogDebug("Enumerating FTP files async from '{Path}' with pattern '{Pattern}'", path, searchPattern);

        var files = await Task.Run(() => GetFiles(path, searchPattern), cancellationToken);
        
        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return file;
        }

        Logger.LogTrace("Completed enumerating FTP files from '{Path}'", path);
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
            Logger.LogTrace("Resolved FTP full path for '{Path}': '{FullPath}'", path, fullPath);
            return fullPath;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resolving FTP full path for '{Path}'", path);
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
            Logger.LogTrace("Resolved FTP directory name for '{Path}': '{DirectoryName}'", path, directoryName ?? "<null>");
            return directoryName;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resolving FTP directory name for '{Path}'", path);
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
            Logger.LogTrace("Resolved FTP file name for '{Path}': '{FileName}'", path, fileName);
            return fileName;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resolving FTP file name for '{Path}'", path);
            throw;
        }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Creates and configures an FtpWebRequest for the specified path and method.
    /// </summary>
    /// <param name="path">The FTP path for the request.</param>
    /// <param name="method">The FTP method to use.</param>
    /// <returns>A configured FtpWebRequest.</returns>
    private FtpWebRequest CreateFtpWebRequest(string path, string method)
    {
        var normalizedPath = path.Replace('\\', '/');
        if (!normalizedPath.StartsWith('/'))
        {
            normalizedPath = '/' + normalizedPath;
        }

        var uri = $"{_options.ServerUri.TrimEnd('/')}{normalizedPath}";
        var request = (FtpWebRequest)WebRequest.Create(uri);
        
        request.Method = method;
        request.Credentials = new NetworkCredential(_options.Username, _options.Password);
        request.UsePassive = _options.UsePassive;
        request.UseBinary = _options.UseBinary;
        request.KeepAlive = _options.KeepAlive;
        request.Timeout = _options.TimeoutMilliseconds;
        
        if (_options.UseSsl)
        {
            request.EnableSsl = true;
        }

        Logger.LogTrace("Created FTP request: {Method} {Uri}", method, uri);
        return request;
    }

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