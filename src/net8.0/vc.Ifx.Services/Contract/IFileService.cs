namespace vc.Ifx.Services.Contract;

/// <summary>
/// Defines operations for file interactions.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Checks if a file exists at the specified path.
    /// </summary>
    bool FileExists(string path);

    /// <summary>
    /// Checks if a file exists.
    /// </summary>
    bool FileExists(FileInfo fileInfo);

    /// <summary>
    /// Reads all text from a file.
    /// </summary>
    string ReadAllText(string path);

    /// <summary>
    /// Reads all text from a file.
    /// </summary>
    string ReadAllText(FileInfo fileInfo);

    /// <summary>
    /// Reads all bytes from a file.
    /// </summary>
    byte[] ReadAllBytes(string path);

    /// <summary>
    /// Reads all lines from a file.
    /// </summary>
    string[] ReadAllLines(string path);

    /// <summary>
    /// Asynchronously reads all text from a file.
    /// </summary>
    Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously reads all text from a file.
    /// </summary>
    Task<string> ReadAllTextAsync(FileInfo fileInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously reads all bytes from a file.
    /// </summary>
    Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes text to a file.
    /// </summary>
    void WriteAllText(string path, string content);

    /// <summary>
    /// Writes bytes to a file.
    /// </summary>
    void WriteAllBytes(string path, byte[] bytes);

    /// <summary>
    /// Asynchronously writes text to a file.
    /// </summary>
    Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously writes bytes to a file.
    /// </summary>
    Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file if it exists.
    /// </summary>
    void DeleteFile(string path);

    /// <summary>
    /// Copies a file to a new location.
    /// </summary>
    void CopyFile(string sourceFileName, string destFileName, bool overwrite = false);

    /// <summary>
    /// Moves a file to a new location.
    /// </summary>
    void MoveFile(string sourceFileName, string destFileName, bool overwrite = false);

    /// <summary>
    /// Creates or opens a file for writing.
    /// </summary>
    Stream OpenWrite(string path);

    /// <summary>
    /// Creates or opens a file for reading.
    /// </summary>
    Stream OpenRead(string path);

    /// <summary>
    /// Gets file information.
    /// </summary>
    FileInfo GetFileInfo(string path);

    /// <summary>
    /// Checks if a file is empty.
    /// </summary>
    bool IsFileEmpty(string path);

    /// <summary>
    /// Checks if a file is empty.
    /// </summary>
    bool IsFileEmpty(FileInfo fileInfo);
}