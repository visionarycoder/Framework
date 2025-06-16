namespace vc.Ifx.Services.Files;

public interface IFileReadAllText
{
    /// <summary>
    /// Reads all text from a file.
    /// </summary>
    string ReadAllText(string path);

    /// <summary>
    /// Reads all text from a file.
    /// </summary>
    string ReadAllText(FileInfo fileInfo);

    /// <summary>
    /// Asynchronously reads all text from a file.
    /// </summary>
    Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously reads all text from a file.
    /// </summary>
    Task<string> ReadAllTextAsync(FileInfo fileInfo, CancellationToken cancellationToken = default);
}