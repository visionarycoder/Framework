using Microsoft.Extensions.Logging;
using Moq;
using VisionaryCoder.Framework.Services.Abstractions;
using VisionaryCoder.Framework.Services.FileSystem;
using Xunit;
using FluentAssertions;

namespace VisionaryCoder.Framework.Tests.Services;

/// <summary>
/// Example unit tests demonstrating how to mock and test the IFileSystem interface.
/// These tests show the improved testability that the consolidated interface provides.
/// </summary>
public class FileSystemServiceTests
{
    private readonly Mock<ILogger<FileSystemService>> _mockLogger;
    private readonly Mock<IFileSystem> _mockFileSystem;

    public FileSystemServiceTests()
    {
        _mockLogger = new Mock<ILogger<FileSystemService>>();
        _mockFileSystem = new Mock<IFileSystem>();
    }

    [Fact]
    public void FileExists_WhenFileExists_ReturnsTrue()
    {
        // Arrange
        const string testPath = @"c:\test\file.txt";
        _mockFileSystem.Setup(fs => fs.FileExists(testPath)).Returns(true);

        // Act
        var result = _mockFileSystem.Object.FileExists(testPath);

        // Assert
        result.Should().BeTrue();
        _mockFileSystem.Verify(fs => fs.FileExists(testPath), Times.Once);
    }

    [Fact]
    public async Task ReadAllTextAsync_WhenFileExists_ReturnsContent()
    {
        // Arrange
        const string testPath = @"c:\test\file.txt";
        const string expectedContent = "Hello, World!";
        _mockFileSystem.Setup(fs => fs.ReadAllTextAsync(testPath, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedContent);

        // Act
        var result = await _mockFileSystem.Object.ReadAllTextAsync(testPath);

        // Assert
        result.Should().Be(expectedContent);
        _mockFileSystem.Verify(fs => fs.ReadAllTextAsync(testPath, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WriteAllTextAsync_WhenCalled_WritesContent()
    {
        // Arrange
        const string testPath = @"c:\test\file.txt";
        const string content = "Hello, World!";
        _mockFileSystem.Setup(fs => fs.WriteAllTextAsync(testPath, content, It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask);

        // Act
        await _mockFileSystem.Object.WriteAllTextAsync(testPath, content);

        // Assert
        _mockFileSystem.Verify(fs => fs.WriteAllTextAsync(testPath, content, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void DirectoryExists_WhenDirectoryExists_ReturnsTrue()
    {
        // Arrange
        const string testPath = @"c:\test\directory";
        _mockFileSystem.Setup(fs => fs.DirectoryExists(testPath)).Returns(true);

        // Act
        var result = _mockFileSystem.Object.DirectoryExists(testPath);

        // Assert
        result.Should().BeTrue();
        _mockFileSystem.Verify(fs => fs.DirectoryExists(testPath), Times.Once);
    }

    [Fact]
    public void GetFiles_WhenDirectoryHasFiles_ReturnsFileArray()
    {
        // Arrange
        const string testPath = @"c:\test\directory";
        const string searchPattern = "*.txt";
        var expectedFiles = new[] { @"c:\test\directory\file1.txt", @"c:\test\directory\file2.txt" };
        
        _mockFileSystem.Setup(fs => fs.GetFiles(testPath, searchPattern))
                      .Returns(expectedFiles);

        // Act
        var result = _mockFileSystem.Object.GetFiles(testPath, searchPattern);

        // Assert
        result.Should().BeEquivalentTo(expectedFiles);
        _mockFileSystem.Verify(fs => fs.GetFiles(testPath, searchPattern), Times.Once);
    }

    [Fact]
    public async Task EnumerateFilesAsync_WhenDirectoryHasFiles_ReturnsAsyncEnumerable()
    {
        // Arrange
        const string testPath = @"c:\test\directory";
        const string searchPattern = "*.txt";
        var expectedFiles = new[] { @"c:\test\directory\file1.txt", @"c:\test\directory\file2.txt" };
        
        _mockFileSystem.Setup(fs => fs.EnumerateFilesAsync(testPath, searchPattern, It.IsAny<CancellationToken>()))
                      .Returns(expectedFiles.ToAsyncEnumerable());

        // Act
        var results = new List<string>();
        await foreach (var file in _mockFileSystem.Object.EnumerateFilesAsync(testPath, searchPattern))
        {
            results.Add(file);
        }

        // Assert
        results.Should().BeEquivalentTo(expectedFiles);
        _mockFileSystem.Verify(fs => fs.EnumerateFilesAsync(testPath, searchPattern, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Example of testing a service that uses IFileSystem as an accessor.
    /// This demonstrates the VBD pattern where an Accessor (FileSystem) is used by higher-level components.
    /// </summary>
    [Fact]
    public async Task ExampleService_UsingFileSystemAccessor_CanBeEasilyTested()
    {
        // Arrange
        var mockFileSystem = new Mock<IFileSystem>();
        const string configPath = @"c:\config\app.json";
        const string configContent = """{"setting": "value"}""";

        mockFileSystem.Setup(fs => fs.FileExists(configPath)).Returns(true);
        mockFileSystem.Setup(fs => fs.ReadAllTextAsync(configPath, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(configContent);

        var service = new ExampleConfigurationService(mockFileSystem.Object);

        // Act
        var result = await service.LoadConfigurationAsync(configPath);

        // Assert
        result.Should().Be(configContent);
        mockFileSystem.Verify(fs => fs.FileExists(configPath), Times.Once);
        mockFileSystem.Verify(fs => fs.ReadAllTextAsync(configPath, It.IsAny<CancellationToken>()), Times.Once);
    }
}

/// <summary>
/// Example service that uses IFileSystem as an accessor.
/// This demonstrates how easy it is to test services that depend on file system operations.
/// </summary>
public class ExampleConfigurationService
{
    private readonly IFileSystem _fileSystem;

    public ExampleConfigurationService(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public async Task<string> LoadConfigurationAsync(string configPath)
    {
        if (!_fileSystem.FileExists(configPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {configPath}");
        }

        return await _fileSystem.ReadAllTextAsync(configPath);
    }
}