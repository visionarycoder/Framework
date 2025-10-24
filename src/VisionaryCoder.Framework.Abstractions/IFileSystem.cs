namespace VisionaryCoder.Framework.Abstractions.Services;

/// <summary>
/// Defines a comprehensive contract for file system operations following Microsoft I/O patterns.
/// This interface consolidates both file and directory operations for improved testability
/// and follows the accessor pattern for VBD (Volatility-Based Decomposition) architecture.
/// </summary>
/// <remarks>
/// This interface is designed to be easily mockable for unit testing and provides
/// both synchronous and asynchronous operations for maximum flexibility.
/// Based on Microsoft's System.IO.Abstractions patterns.
/// </remarks>
public interface IFileSystemProvider
{
    // File Operations
    
    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="path">The file path to check.</param>
    /// <returns>true if the file exists; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when path is null or whitespace.</exception>
    bool FileExists(string path);
    /// <param name="fileInfo">The FileInfo object representing the file to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when fileInfo is null.</exception>
    bool FileExists(FileInfo fileInfo);
    /// Reads all text from a file synchronously.
    /// <param name="path">The file path to read from.</param>
    /// <returns>The file contents as a string.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
    string ReadAllText(string path);
    /// Reads all text from a file asynchronously.
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with the file contents.</returns>
    Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);
    /// Reads all bytes from a file synchronously.
    /// <returns>The file contents as a byte array.</returns>
    byte[] ReadAllBytes(string path);
    /// Reads all bytes from a file asynchronously.
    Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);
    /// Writes text to a file synchronously, creating the file if it doesn't exist.
    /// <param name="path">The file path to write to.</param>
    /// <param name="content">The content to write.</param>
    /// <exception cref="ArgumentNullException">Thrown when content is null.</exception>
    void WriteAllText(string path, string content);
    /// Writes text to a file asynchronously, creating the file if it doesn't exist.
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default);
    /// Writes bytes to a file synchronously, creating the file if it doesn't exist.
    /// <param name="bytes">The bytes to write.</param>
    /// <exception cref="ArgumentNullException">Thrown when bytes is null.</exception>
    void WriteAllBytes(string path, byte[] bytes);
    /// Writes bytes to a file asynchronously, creating the file if it doesn't exist.
    Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);
    /// Deletes the specified file if it exists.
    /// <param name="path">The file path to delete.</param>
    void DeleteFile(string path);
    /// Deletes the specified file asynchronously if it exists.
    Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);
    // Directory Operations
    /// Determines whether the specified directory exists.
    /// <param name="path">The directory path to check.</param>
    /// <returns>true if the directory exists; otherwise, false.</returns>
    bool DirectoryExists(string path);
    /// Creates a directory at the specified path, including any necessary parent directories.
    /// <param name="path">The directory path to create.</param>
    /// <returns>A DirectoryInfo object representing the created directory.</returns>
    DirectoryInfo CreateDirectory(string path);
    /// Creates a directory at the specified path asynchronously, including any necessary parent directories.
    /// <returns>A task representing the asynchronous operation with the created DirectoryInfo.</returns>
    Task<DirectoryInfo> CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);
    /// Deletes the specified directory and optionally all its contents.
    /// <param name="path">The directory path to delete.</param>
    /// <param name="recursive">true to delete the directory and all its contents; otherwise, false.</param>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist and recursive is false.</exception>
    void DeleteDirectory(string path, bool recursive = true);
    /// Deletes the specified directory and optionally all its contents asynchronously.
    Task DeleteDirectoryAsync(string path, bool recursive = true, CancellationToken cancellationToken = default);
    /// Gets the names of files in the specified directory.
    /// <param name="path">The directory path to search.</param>
    /// <param name="searchPattern">The search pattern to match file names against.</param>
    /// <returns>An array of file names in the directory.</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
    string[] GetFiles(string path, string searchPattern = "*");
    /// Gets the names of directories in the specified directory.
    /// <param name="searchPattern">The search pattern to match directory names against.</param>
    /// <returns>An array of directory names in the directory.</returns>
    string[] GetDirectories(string path, string searchPattern = "*");
    /// Enumerates files in the specified directory asynchronously.
    /// <returns>An async enumerable of file names in the directory.</returns>
    IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern = "*", CancellationToken cancellationToken = default);
    /// Gets the full path for the specified relative path.
    /// <param name="path">The relative or absolute path.</param>
    /// <returns>The full path.</returns>
    string GetFullPath(string path);
    /// Gets the directory name from the specified path.
    /// <param name="path">The file or directory path.</param>
    /// <returns>The directory name, or null if path is a root directory.</returns>
    string? GetDirectoryName(string path);
    /// Gets the file name from the specified path.
    /// <param name="path">The file path.</param>
    /// <returns>The file name including extension.</returns>
    string GetFileName(string path);
}
