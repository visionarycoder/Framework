// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Logging;

namespace VisionaryCoder.Framework.Storage;

/// <summary>
/// Provides storage operations for files and data.
/// </summary>
public class StorageService(ILogger<StorageService> logger) : ServiceBase<StorageService>(logger)
{

    private readonly ILogger<StorageService> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    // File operations
    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public bool FileExists(FileInfo fileInfo)
    {
        return fileInfo?.Exists ?? false;
    }

    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }

    public async Task<string> ReadAllTextAsync(string path)
    {
        return await File.ReadAllTextAsync(path);
    }

    public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken)
    {
        return await File.ReadAllTextAsync(path, cancellationToken);
    }

    public byte[] ReadAllBytes(string path)
    {
        return File.ReadAllBytes(path);
    }

    public async Task<byte[]> ReadAllBytesAsync(string path)
    {
        return await File.ReadAllBytesAsync(path);
    }

    public void WriteAllText(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    public async Task WriteAllTextAsync(string path, string content)
    {
        await File.WriteAllTextAsync(path, content);
    }

    public void WriteAllBytes(string path, byte[] bytes)
    {
        File.WriteAllBytes(path, bytes);
    }

    public async Task WriteAllBytesAsync(string path, byte[] bytes)
    {
        await File.WriteAllBytesAsync(path, bytes);
    }

    public void DeleteFile(string path)
    {
        File.Delete(path);
    }

    public Task DeleteFileAsync(string path)
    {
        File.Delete(path);
        return Task.CompletedTask;
    }

    // Directory operations
    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public DirectoryInfo CreateDirectory(string path)
    {
        return Directory.CreateDirectory(path);
    }

    public Task<DirectoryInfo> CreateDirectoryAsync(string path)
    {
        return Task.FromResult(Directory.CreateDirectory(path));
    }

    public void DeleteDirectory(string path, bool recursive = false)
    {
        Directory.Delete(path, recursive);
    }

    public Task DeleteDirectoryAsync(string path, bool recursive = false)
    {
        Directory.Delete(path, recursive);
        return Task.CompletedTask;
    }

    public string[] GetFiles(string path)
    {
        return Directory.GetFiles(path);
    }

    public string[] GetFiles(string path, string searchPattern)
    {
        return Directory.GetFiles(path, searchPattern);
    }

    public string[] GetDirectories(string path)
    {
        return Directory.GetDirectories(path);
    }

    public string[] GetDirectories(string path, string searchPattern)
    {
        return Directory.GetDirectories(path, searchPattern);
    }

    public async IAsyncEnumerable<string> EnumerateFilesAsync(string path)
    {
        await Task.Yield();
        foreach (string file in Directory.EnumerateFiles(path))
        {
            yield return file;
        }
    }

    public async IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern)
    {
        await Task.Yield();
        foreach (string file in Directory.EnumerateFiles(path, searchPattern))
        {
            yield return file;
        }
    }

    public async IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        foreach (string file in Directory.EnumerateFiles(path, searchPattern))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return file;
        }
    }

    // Path operations
    public string GetFullPath(string path)
    {
        return Path.GetFullPath(path);
    }

    public string? GetDirectoryName(string path)
    {
        return Path.GetDirectoryName(path);
    }

    public string? GetFileName(string path)
    {
        return Path.GetFileName(path);
    }
}
