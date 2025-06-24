namespace vc.Ifx.Services;

/// <summary>
/// Implementation of IFileService that interacts with the actual file system.
/// </summary>
public class FileService : IFileService
{
    public bool FileExists(string path) => File.Exists(path);

    public bool FileExists(FileInfo fileInfo) => fileInfo.Exists;

    public string ReadAllText(string path) => File.ReadAllText(path);

    public string ReadAllText(FileInfo fileInfo) => File.ReadAllText(fileInfo.FullName);

    public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
        => File.ReadAllTextAsync(path, cancellationToken);

    public Task<string> ReadAllTextAsync(FileInfo fileInfo, CancellationToken cancellationToken = default)
        => File.ReadAllTextAsync(fileInfo.FullName, cancellationToken);

    public void WriteAllText(string path, string content)
        => File.WriteAllText(path, content);

    public Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
        => File.WriteAllTextAsync(path, content, cancellationToken);

    public void DeleteFile(string path)
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public bool IsFileEmpty(string path)
    {
        var fileInfo = new FileInfo(path);
        return fileInfo is { Exists: true, Length: 0 };
    }

    public bool IsFileEmpty(FileInfo fileInfo) => fileInfo is { Exists: true, Length: 0 };
}