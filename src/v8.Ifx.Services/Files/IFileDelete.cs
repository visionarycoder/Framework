namespace vc.Ifx.Services.Files;

public interface IFileDelete
{
    /// <summary>
    /// Deletes a file if it exists.
    /// </summary>
    void DeleteFile(string path);

    /// <summary>
    /// Deletes a file if it exists.
    /// </summary>
    void DeleteFile(FileInfo fileInfo);
    
    /// <summary>
    /// Asynchronously deletes a file if it exists.
    /// </summary>
    Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Asynchronously deletes a file if it exists.
    /// </summary>
    Task DeleteFileAsync(FileInfo fileInfo, CancellationToken cancellationToken = default);
}