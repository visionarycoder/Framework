using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Abstractions;
using VisionaryCoder.Framework.Services.Abstractions;

namespace VisionaryCoder.Framework.Services.FileSystem;

/// <summary>
/// Provides comprehensive file system operations implementation following Microsoft I/O patterns.
/// This service consolidates both file and directory operations with logging, error handling, and async support.
/// Designed for use in accessor components within VBD (Volatility-Based Decomposition) architecture.
/// </summary>
public sealed class FileSystemService : ServiceBase<FileSystemService>, IFileSystem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for this service.</param>
    public FileSystemService(ILogger<FileSystemService> logger) : base(logger)
    {
    }

    #region File Operations

    /// <inheritdoc />
    public bool FileExists(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        try
        {
            var exists = File.Exists(path);
            Logger.LogTrace("File existence check for '{Path}': {Exists}", path, exists);
            return exists;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking file existence for '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public bool FileExists(FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        
        try
        {
            fileInfo.Refresh(); // Ensure we have current information
            var exists = fileInfo.Exists;
            Logger.LogTrace("File existence check for '{Path}': {Exists}", fileInfo.FullName, exists);
            return exists;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking file existence for '{Path}'", fileInfo.FullName);
            throw;
        }
    }

    /// <inheritdoc />
    public string ReadAllText(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Reading all text from '{Path}'", path);
            var content = File.ReadAllText(path);
            Logger.LogTrace("Successfully read {Length} characters from '{Path}'", content.Length, path);
            return content;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reading text from '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Reading all text async from '{Path}'", path);
            var content = await File.ReadAllTextAsync(path, cancellationToken);
            Logger.LogTrace("Successfully read {Length} characters from '{Path}'", content.Length, path);
            return content;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reading text async from '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public byte[] ReadAllBytes(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Reading all bytes from '{Path}'", path);
            var bytes = File.ReadAllBytes(path);
            Logger.LogTrace("Successfully read {Length} bytes from '{Path}'", bytes.Length, path);
            return bytes;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reading bytes from '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Reading all bytes async from '{Path}'", path);
            var bytes = await File.ReadAllBytesAsync(path, cancellationToken);
            Logger.LogTrace("Successfully read {Length} bytes from '{Path}'", bytes.Length, path);
            return bytes;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reading bytes async from '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public void WriteAllText(string path, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(content);

        try
        {
            Logger.LogDebug("Writing {Length} characters to '{Path}'", content.Length, path);
            File.WriteAllText(path, content);
            Logger.LogTrace("Successfully wrote text to '{Path}'", path);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error writing text to '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(content);

        try
        {
            Logger.LogDebug("Writing {Length} characters async to '{Path}'", content.Length, path);
            await File.WriteAllTextAsync(path, content, cancellationToken);
            Logger.LogTrace("Successfully wrote text async to '{Path}'", path);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error writing text async to '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public void WriteAllBytes(string path, byte[] bytes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(bytes);

        try
        {
            Logger.LogDebug("Writing {Length} bytes to '{Path}'", bytes.Length, path);
            File.WriteAllBytes(path, bytes);
            Logger.LogTrace("Successfully wrote bytes to '{Path}'", path);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error writing bytes to '{Path}'", path);
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
            Logger.LogDebug("Writing {Length} bytes async to '{Path}'", bytes.Length, path);
            await File.WriteAllBytesAsync(path, bytes, cancellationToken);
            Logger.LogTrace("Successfully wrote bytes async to '{Path}'", path);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error writing bytes async to '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public void DeleteFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            if (File.Exists(path))
            {
                Logger.LogDebug("Deleting file '{Path}'", path);
                File.Delete(path);
                Logger.LogTrace("Successfully deleted file '{Path}'", path);
            }
            else
            {
                Logger.LogTrace("File '{Path}' does not exist, no deletion needed", path);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting file '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
    {
        // File.Delete is not I/O bound, so we run it in a task for consistency
        return Task.Run(() => DeleteFile(path), cancellationToken);
    }

    #endregion

    #region Directory Operations

    /// <inheritdoc />
    public bool DirectoryExists(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        try
        {
            var exists = Directory.Exists(path);
            Logger.LogTrace("Directory existence check for '{Path}': {Exists}", path, exists);
            return exists;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking directory existence for '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public DirectoryInfo CreateDirectory(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            Logger.LogDebug("Creating directory '{Path}'", path);
            var directoryInfo = Directory.CreateDirectory(path);
            Logger.LogTrace("Successfully created directory '{Path}'", path);
            return directoryInfo;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating directory '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public Task<DirectoryInfo> CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
    {
        // Directory.CreateDirectory is not I/O bound, so we run it in a task for consistency
        return Task.Run(() => CreateDirectory(path), cancellationToken);
    }

    /// <inheritdoc />
    public void DeleteDirectory(string path, bool recursive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            if (Directory.Exists(path))
            {
                Logger.LogDebug("Deleting directory '{Path}' (recursive: {Recursive})", path, recursive);
                Directory.Delete(path, recursive);
                Logger.LogTrace("Successfully deleted directory '{Path}'", path);
            }
            else
            {
                Logger.LogTrace("Directory '{Path}' does not exist, no deletion needed", path);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting directory '{Path}' (recursive: {Recursive})", path, recursive);
            throw;
        }
    }

    /// <inheritdoc />
    public Task DeleteDirectoryAsync(string path, bool recursive = true, CancellationToken cancellationToken = default)
    {
        // Directory.Delete is not I/O bound, so we run it in a task for consistency
        return Task.Run(() => DeleteDirectory(path, recursive), cancellationToken);
    }

    /// <inheritdoc />
    public string[] GetFiles(string path, string searchPattern = "*")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(searchPattern);

        try
        {
            Logger.LogDebug("Getting files from '{Path}' with pattern '{Pattern}'", path, searchPattern);
            var files = Directory.GetFiles(path, searchPattern);
            Logger.LogTrace("Found {Count} files in '{Path}'", files.Length, path);
            return files;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting files from '{Path}' with pattern '{Pattern}'", path, searchPattern);
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
            Logger.LogDebug("Getting directories from '{Path}' with pattern '{Pattern}'", path, searchPattern);
            var directories = Directory.GetDirectories(path, searchPattern);
            Logger.LogTrace("Found {Count} directories in '{Path}'", directories.Length, path);
            return directories;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting directories from '{Path}' with pattern '{Pattern}'", path, searchPattern);
            throw;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern = "*", [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(searchPattern);

        Logger.LogDebug("Enumerating files async from '{Path}' with pattern '{Pattern}'", path, searchPattern);

        await Task.Yield(); // Make it actually async

        foreach (var file in Directory.EnumerateFiles(path, searchPattern))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return file;
        }

        Logger.LogTrace("Completed enumerating files from '{Path}'", path);
    }

    #endregion

    #region Path Utilities

    /// <inheritdoc />
    public string GetFullPath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            var fullPath = Path.GetFullPath(path);
            Logger.LogTrace("Resolved full path for '{Path}': '{FullPath}'", path, fullPath);
            return fullPath;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resolving full path for '{Path}'", path);
            throw;
        }
    }

    /// <inheritdoc />
    public string? GetDirectoryName(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            var directoryName = Path.GetDirectoryName(path);
            Logger.LogTrace("Resolved directory name for '{Path}': '{DirectoryName}'", path, directoryName ?? "<null>");
            return directoryName;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resolving directory name for '{Path}'", path);
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
            Logger.LogTrace("Resolved file name for '{Path}': '{FileName}'", path, fileName);
            return fileName;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resolving file name for '{Path}'", path);
            throw;
        }
    }

    #endregion
}