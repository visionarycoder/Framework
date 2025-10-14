using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Abstractions;
using VisionaryCoder.Framework.Services.Abstractions;

namespace VisionaryCoder.Framework.Services.FileSystem;

/// <summary>
/// Provides file system operations implementation following Microsoft I/O patterns.
/// This service wraps System.IO operations with logging, error handling, and async support.
/// </summary>
public sealed class FileService : ServiceBase<FileService>, IFileService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for this service.</param>
    public FileService(ILogger<FileService> logger) : base(logger)
    {
    }

    /// <inheritdoc />
    public bool Exists(string path)
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
    public bool Exists(FileInfo fileInfo)
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
    public void Delete(string path)
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
    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        // File.Delete is not I/O bound, so we run it in a task for consistency
        return Task.Run(() => Delete(path), cancellationToken);
    }
}
