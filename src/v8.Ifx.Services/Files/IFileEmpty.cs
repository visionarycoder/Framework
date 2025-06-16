namespace vc.Ifx.Services.Files;

public interface IFileEmpty
{
    /// <summary>
    /// Checks if a file is empty.
    /// </summary>
    bool IsFileEmpty(string path);

    /// <summary>
    /// Checks if a file is empty.
    /// </summary>
    bool IsFileEmpty(FileInfo fileInfo);

    /// <summary>
    /// Checks if a file is empty.
    /// </summary>
    Task<bool> IsFileEmptyAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a file is empty.
    /// </summary>
    Task<bool> IsFileEmptyAsync(FileInfo fileInfo, CancellationToken cancellationToken = default);
}