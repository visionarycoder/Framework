namespace vc.Ifx.Services.Files;

public interface IFileExists
{
    /// <summary>
    /// Checks if a file exists at the specified path.
    /// </summary>
    bool FileExists(string path);

    /// <summary>
    /// Checks if the specified file exists.
    /// </summary>
    bool FileExists(FileInfo fileInfo);

    /// <summary>
    /// Checks if a file exists at the specified path.
    /// </summary>
    bool FileExistsAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the specified file exists.
    /// </summary>
    bool FileExistsAsync(FileInfo fileInfo, CancellationToken cancellationToken = default);

}