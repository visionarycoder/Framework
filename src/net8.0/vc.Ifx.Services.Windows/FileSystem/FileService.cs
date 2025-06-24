namespace vc.Ifx.Services.FileSystem;

/// <summary>
/// Implementation of IFileSystemService that interacts with the actual file system.
/// </summary>
public class FileSystemService : IFileSystemService
{
    #region IFileService Implementation

    public bool FileExists(string path) => File.Exists(path);

    public bool FileExists(FileInfo fileInfo) => fileInfo.Exists;

    public string ReadAllText(string path) => File.ReadAllText(path);

    public string ReadAllText(FileInfo fileInfo) => File.ReadAllText(fileInfo.FullName);

    public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);

    public string[] ReadAllLines(string path) => File.ReadAllLines(path);

    public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
        => File.ReadAllTextAsync(path, cancellationToken);

    public Task<string> ReadAllTextAsync(FileInfo fileInfo, CancellationToken cancellationToken = default)
        => File.ReadAllTextAsync(fileInfo.FullName, cancellationToken);

    public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        => File.ReadAllBytesAsync(path, cancellationToken);

    public void WriteAllText(string path, string content)
        => File.WriteAllText(path, content);

    public void WriteAllBytes(string path, byte[] bytes)
        => File.WriteAllBytes(path, bytes);

    public Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
        => File.WriteAllTextAsync(path, content, cancellationToken);

    public Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
        => File.WriteAllBytesAsync(path, bytes, cancellationToken);

    public void DeleteFile(string path)
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public void CopyFile(string sourceFileName, string destFileName, bool overwrite = false)
        => File.Copy(sourceFileName, destFileName, overwrite);

    public void MoveFile(string sourceFileName, string destFileName, bool overwrite = false)
    {
        if(overwrite && File.Exists(destFileName))
        {
            File.Delete(destFileName);
        }
        File.Move(sourceFileName, destFileName);
    }

    public Stream OpenWrite(string path) => File.OpenWrite(path);

    public Stream OpenRead(string path) => File.OpenRead(path);

    public FileInfo GetFileInfo(string path) => new FileInfo(path);

    public bool IsFileEmpty(string path) => new FileInfo(path) is { Exists: true, Length: 0 };

    public bool IsFileEmpty(FileInfo fileInfo) => fileInfo is { Exists: true, Length: 0 };

    #endregion

    #region IDirectoryService Implementation

    public bool DirectoryExists(string path) => Directory.Exists(path);

    public bool DirectoryExists(DirectoryInfo directoryInfo) => directoryInfo.Exists;

    public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);

    public void DeleteDirectory(string path, bool recursive = false)
    {
        if(Directory.Exists(path))
        {
            Directory.Delete(path, recursive);
        }
    }

    public IEnumerable<FileInfo> GetFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly) => new DirectoryInfo(path).GetFiles(searchPattern, searchOption);

    public IEnumerable<DirectoryInfo> GetDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly) => new DirectoryInfo(path).GetDirectories(searchPattern, searchOption);

    public void MoveDirectory(string sourceDirName, string destDirName) => Directory.Move(sourceDirName, destDirName);

    public DirectoryInfo GetDirectoryInfo(string path) => new DirectoryInfo(path);

    public string GetCurrentDirectory() => Directory.GetCurrentDirectory();

    public void SetCurrentDirectory(string path) => Directory.SetCurrentDirectory(path);

    #endregion
}