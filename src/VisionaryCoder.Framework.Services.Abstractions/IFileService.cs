namespace VisionaryCoder.Framework.Services.Abstractions;

/// <summary>
/// Defines contract for file system operations following Microsoft I/O patterns.
/// Provides both synchronous and asynchronous methods for file manipulation.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="path">The file path to check.</param>
    /// <returns>true if the file exists; otherwise, false.</returns>
    bool Exists(string path);

    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="fileInfo">The FileInfo object representing the file to check.</param>
    /// <returns>true if the file exists; otherwise, false.</returns>
    bool Exists(FileInfo fileInfo);

    /// <summary>
    /// Reads all text from a file synchronously.
    /// </summary>
    /// <param name="path">The file path to read from.</param>
    /// <returns>The file contents as a string.</returns>
    string ReadAllText(string path);

    /// <summary>
    /// Reads all text from a file asynchronously.
    /// </summary>
    /// <param name="path">The file path to read from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with the file contents.</returns>
    Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads all bytes from a file synchronously.
    /// </summary>
    /// <param name="path">The file path to read from.</param>
    /// <returns>The file contents as a byte array.</returns>
    byte[] ReadAllBytes(string path);

    /// <summary>
    /// Reads all bytes from a file asynchronously.
    /// </summary>
    /// <param name="path">The file path to read from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with the file contents.</returns>
    Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes text to a file synchronously, creating the file if it doesn't exist.
    /// </summary>
    /// <param name="path">The file path to write to.</param>
    /// <param name="content">The content to write.</param>
    void WriteAllText(string path, string content);

    /// <summary>
    /// Writes text to a file asynchronously, creating the file if it doesn't exist.
    /// </summary>
    /// <param name="path">The file path to write to.</param>
    /// <param name="content">The content to write.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes bytes to a file synchronously, creating the file if it doesn't exist.
    /// </summary>
    /// <param name="path">The file path to write to.</param>
    /// <param name="bytes">The bytes to write.</param>
    void WriteAllBytes(string path, byte[] bytes);

    /// <summary>
    /// Writes bytes to a file asynchronously, creating the file if it doesn't exist.
    /// </summary>
    /// <param name="path">The file path to write to.</param>
    /// <param name="bytes">The bytes to write.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified file if it exists.
    /// </summary>
    /// <param name="path">The file path to delete.</param>
    void Delete(string path);

    /// <summary>
    /// Deletes the specified file asynchronously if it exists.
    /// </summary>
    /// <param name="path">The file path to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);
}
