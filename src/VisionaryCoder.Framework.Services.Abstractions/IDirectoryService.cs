namespace VisionaryCoder.Framework.Services.Abstractions;

/// <summary>
/// Defines contract for directory operations following Microsoft I/O patterns.
/// Provides both synchronous and asynchronous methods for directory manipulation.
/// </summary>
public interface IDirectoryService
{
    /// <summary>
    /// Determines whether the specified directory exists.
    /// </summary>
    /// <param name="path">The directory path to check.</param>
    /// <returns>true if the directory exists; otherwise, false.</returns>
    bool Exists(string path);

    /// <summary>
    /// Creates a directory at the specified path, including any necessary parent directories.
    /// </summary>
    /// <param name="path">The directory path to create.</param>
    /// <returns>A DirectoryInfo object representing the created directory.</returns>
    DirectoryInfo Create(string path);

    /// <summary>
    /// Creates a directory at the specified path asynchronously, including any necessary parent directories.
    /// </summary>
    /// <param name="path">The directory path to create.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with the created DirectoryInfo.</returns>
    Task<DirectoryInfo> CreateAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified directory and all its contents.
    /// </summary>
    /// <param name="path">The directory path to delete.</param>
    /// <param name="recursive">true to delete the directory and all its contents; otherwise, false.</param>
    void Delete(string path, bool recursive = true);

    /// <summary>
    /// Deletes the specified directory and all its contents asynchronously.
    /// </summary>
    /// <param name="path">The directory path to delete.</param>
    /// <param name="recursive">true to delete the directory and all its contents; otherwise, false.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(string path, bool recursive = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the names of files in the specified directory.
    /// </summary>
    /// <param name="path">The directory path to search.</param>
    /// <param name="searchPattern">The search pattern to match file names against.</param>
    /// <returns>An array of file names in the directory.</returns>
    string[] GetFiles(string path, string searchPattern = "*");

    /// <summary>
    /// Gets the names of directories in the specified directory.
    /// </summary>
    /// <param name="path">The directory path to search.</param>
    /// <param name="searchPattern">The search pattern to match directory names against.</param>
    /// <returns>An array of directory names in the directory.</returns>
    string[] GetDirectories(string path, string searchPattern = "*");

    /// <summary>
    /// Enumerates files in the specified directory asynchronously.
    /// </summary>
    /// <param name="path">The directory path to search.</param>
    /// <param name="searchPattern">The search pattern to match file names against.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An async enumerable of file names in the directory.</returns>
    IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern = "*", CancellationToken cancellationToken = default);
}
