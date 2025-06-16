namespace vc.Ifx.Services.Files;

public interface IFileWriteAllText
{
    /// <summary>
    /// Writes text to a file.
    /// </summary>
    bool WriteAllText(string path, string content);

    /// <summary>
    /// Writes text to a file.
    /// </summary>
    bool WriteAllText(FileInfo fileInfo, string content);

    /// <summary>
    /// Asynchronously writes text to a file.
    /// </summary>
    Task<bool> WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes text to a file.
    /// </summary>
    Task<bool> WriteAllTextAsync(FileInfo fileInfo, string content, CancellationToken cancellationToken = default);

}