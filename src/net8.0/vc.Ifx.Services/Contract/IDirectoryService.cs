namespace vc.Ifx.Services.Contract;

/// <summary>
/// Defines operations for directory interactions.
/// </summary>
public interface IDirectoryService
{
    /// <summary>
    /// Checks if a directory exists at the specified path.
    /// </summary>
    bool DirectoryExists(string path);

    /// <summary>
    /// Checks if a directory exists.
    /// </summary>
    bool DirectoryExists(DirectoryInfo directoryInfo);

    /// <summary>
    /// Creates a directory at the specified path.
    /// </summary>
    DirectoryInfo CreateDirectory(string path);

    /// <summary>
    /// Deletes a directory if it exists.
    /// </summary>
    void DeleteDirectory(string path, bool recursive = false);

    /// <summary>
    /// Gets all files in a directory.
    /// </summary>
    IEnumerable<FileInfo> GetFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Gets all directories in a directory.
    /// </summary>
    IEnumerable<DirectoryInfo> GetDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Moves a directory to a new location.
    /// </summary>
    void MoveDirectory(string sourceDirName, string destDirName);

    /// <summary>
    /// Gets directory information.
    /// </summary>
    DirectoryInfo GetDirectoryInfo(string path);

    /// <summary>
    /// Gets the current directory.
    /// </summary>
    string GetCurrentDirectory();

    /// <summary>
    /// Sets the current directory.
    /// </summary>
    void SetCurrentDirectory(string path);
}