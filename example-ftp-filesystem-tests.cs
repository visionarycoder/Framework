using Microsoft.Extensions.Logging;
using Moq;
using VisionaryCoder.Framework.Services.Abstractions;
using VisionaryCoder.Framework.Services.FileSystem;
using Xunit;
using FluentAssertions;

namespace VisionaryCoder.Framework.Tests.Services;

/// <summary>
/// Unit tests for FTP file system service demonstrating testability and mocking capabilities.
/// These tests show how to test FTP operations without requiring an actual FTP server.
/// </summary>
public class FtpFileSystemServiceTests
{
    private readonly Mock<ILogger<FtpFileSystemService>> _mockLogger;
    private readonly Mock<IFileSystem> _mockFtpFileSystem;
    private readonly FtpFileSystemOptions _testOptions;

    public FtpFileSystemServiceTests()
    {
        _mockLogger = new Mock<ILogger<FtpFileSystemService>>();
        _mockFtpFileSystem = new Mock<IFileSystem>();
        
        _testOptions = new FtpFileSystemOptions
        {
            Host = "test.ftp.com",
            Username = "testuser",
            Password = "testpass",
            Port = 21,
            UseSsl = false,
            UsePassive = true
        };
    }

    [Fact]
    public void FtpFileSystemOptions_WhenProperlyConfigured_CreatesCorrectServerUri()
    {
        // Arrange & Act
        var options = new FtpFileSystemOptions
        {
            Host = "example.com",
            Username = "user",
            Password = "pass",
            Port = 21,
            UseSsl = false
        };

        // Assert
        options.ServerUri.Should().Be("ftp://example.com:21");
    }

    [Fact]
    public void FtpFileSystemOptions_WhenUsingSsl_CreatesCorrectFtpsUri()
    {
        // Arrange & Act
        var options = new FtpFileSystemOptions
        {
            Host = "secure.ftp.com",
            Username = "user",
            Password = "pass",
            Port = 990,
            UseSsl = true
        };

        // Assert
        options.ServerUri.Should().Be("ftps://secure.ftp.com:990");
    }

    [Fact]
    public async Task MockedFtpFileExists_WhenFileExists_ReturnsTrue()
    {
        // Arrange
        const string remotePath = "/data/config.json";
        _mockFtpFileSystem.Setup(fs => fs.FileExists(remotePath))
                         .Returns(true);

        // Act
        var result = _mockFtpFileSystem.Object.FileExists(remotePath);

        // Assert
        result.Should().BeTrue();
        _mockFtpFileSystem.Verify(fs => fs.FileExists(remotePath), Times.Once);
    }

    [Fact]
    public async Task MockedFtpReadAllTextAsync_WhenFileExists_ReturnsContent()
    {
        // Arrange
        const string remotePath = "/data/config.json";
        const string expectedContent = """{"server": "production", "timeout": 30}""";
        
        _mockFtpFileSystem.Setup(fs => fs.ReadAllTextAsync(remotePath, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedContent);

        // Act
        var result = await _mockFtpFileSystem.Object.ReadAllTextAsync(remotePath);

        // Assert
        result.Should().Be(expectedContent);
        _mockFtpFileSystem.Verify(fs => fs.ReadAllTextAsync(remotePath, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MockedFtpWriteAllTextAsync_WhenWritingFile_CompletesSuccessfully()
    {
        // Arrange
        const string remotePath = "/logs/application.log";
        const string content = "Application started successfully";
        
        _mockFtpFileSystem.Setup(fs => fs.WriteAllTextAsync(remotePath, content, It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

        // Act
        await _mockFtpFileSystem.Object.WriteAllTextAsync(remotePath, content);

        // Assert
        _mockFtpFileSystem.Verify(fs => fs.WriteAllTextAsync(remotePath, content, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void MockedFtpGetFiles_WhenDirectoryHasFiles_ReturnsFileList()
    {
        // Arrange
        const string remotePath = "/data";
        const string searchPattern = "*.json";
        var expectedFiles = new[]
        {
            "/data/config.json",
            "/data/settings.json",
            "/data/metadata.json"
        };

        _mockFtpFileSystem.Setup(fs => fs.GetFiles(remotePath, searchPattern))
                         .Returns(expectedFiles);

        // Act
        var result = _mockFtpFileSystem.Object.GetFiles(remotePath, searchPattern);

        // Assert
        result.Should().BeEquivalentTo(expectedFiles);
        _mockFtpFileSystem.Verify(fs => fs.GetFiles(remotePath, searchPattern), Times.Once);
    }

    [Fact]
    public void MockedFtpCreateDirectory_WhenCreatingDirectory_ReturnsDirectoryInfo()
    {
        // Arrange
        const string remotePath = "/uploads/2023/10";
        var expectedDirectoryInfo = new DirectoryInfo(remotePath);

        _mockFtpFileSystem.Setup(fs => fs.CreateDirectory(remotePath))
                         .Returns(expectedDirectoryInfo);

        // Act
        var result = _mockFtpFileSystem.Object.CreateDirectory(remotePath);

        // Assert
        result.FullName.Should().Be(expectedDirectoryInfo.FullName);
        _mockFtpFileSystem.Verify(fs => fs.CreateDirectory(remotePath), Times.Once);
    }

    [Fact]
    public async Task MockedFtpEnumerateFilesAsync_WhenEnumerating_ReturnsAsyncFiles()
    {
        // Arrange
        const string remotePath = "/backups";
        const string searchPattern = "*.bak";
        var expectedFiles = new[]
        {
            "/backups/database_20231014.bak",
            "/backups/logs_20231014.bak"
        };

        _mockFtpFileSystem.Setup(fs => fs.EnumerateFilesAsync(remotePath, searchPattern, It.IsAny<CancellationToken>()))
                         .Returns(expectedFiles.ToAsyncEnumerable());

        // Act
        var results = new List<string>();
        await foreach (var file in _mockFtpFileSystem.Object.EnumerateFilesAsync(remotePath, searchPattern))
        {
            results.Add(file);
        }

        // Assert
        results.Should().BeEquivalentTo(expectedFiles);
        _mockFtpFileSystem.Verify(fs => fs.EnumerateFilesAsync(remotePath, searchPattern, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("/data/file.txt", "/data/file.txt")]
    [InlineData("data/file.txt", "/data/file.txt")]
    [InlineData("file.txt", "/file.txt")]
    public void MockedFtpGetFullPath_WithVariousPaths_ReturnsNormalizedPath(string inputPath, string expectedPath)
    {
        // Arrange
        _mockFtpFileSystem.Setup(fs => fs.GetFullPath(inputPath))
                         .Returns(expectedPath);

        // Act
        var result = _mockFtpFileSystem.Object.GetFullPath(inputPath);

        // Assert
        result.Should().Be(expectedPath);
    }

    /// <summary>
    /// Example of testing a service that uses FTP file system as an accessor.
    /// This demonstrates the VBD pattern where an FTP Accessor is used by higher-level components.
    /// </summary>
    [Fact]
    public async Task ExampleRemoteDataService_UsingFtpFileSystemAccessor_CanBeEasilyTested()
    {
        // Arrange
        var mockFtpFileSystem = new Mock<IFileSystem>();
        const string remotePath = "/data/customer_data.csv";
        const string csvContent = "Id,Name,Email\n1,John Doe,john@example.com\n2,Jane Smith,jane@example.com";

        mockFtpFileSystem.Setup(fs => fs.FileExists(remotePath)).Returns(true);
        mockFtpFileSystem.Setup(fs => fs.ReadAllTextAsync(remotePath, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(csvContent);

        var service = new ExampleRemoteDataService(mockFtpFileSystem.Object);

        // Act
        var result = await service.LoadCustomerDataAsync(remotePath);

        // Assert
        result.Should().Contain("John Doe");
        result.Should().Contain("Jane Smith");
        mockFtpFileSystem.Verify(fs => fs.FileExists(remotePath), Times.Once);
        mockFtpFileSystem.Verify(fs => fs.ReadAllTextAsync(remotePath, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExampleRemoteDataService_WhenFileNotExists_ThrowsFileNotFoundException()
    {
        // Arrange
        var mockFtpFileSystem = new Mock<IFileSystem>();
        const string remotePath = "/data/nonexistent.csv";

        mockFtpFileSystem.Setup(fs => fs.FileExists(remotePath)).Returns(false);

        var service = new ExampleRemoteDataService(mockFtpFileSystem.Object);

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => 
            service.LoadCustomerDataAsync(remotePath));

        mockFtpFileSystem.Verify(fs => fs.FileExists(remotePath), Times.Once);
        mockFtpFileSystem.Verify(fs => fs.ReadAllTextAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

/// <summary>
/// Example service that uses IFileSystem (FTP) as an accessor for remote data operations.
/// This demonstrates how easy it is to test services that depend on FTP file system operations.
/// </summary>
public class ExampleRemoteDataService
{
    private readonly IFileSystem _remoteFileSystem;

    public ExampleRemoteDataService(IFileSystem remoteFileSystem)
    {
        _remoteFileSystem = remoteFileSystem ?? throw new ArgumentNullException(nameof(remoteFileSystem));
    }

    public async Task<string> LoadCustomerDataAsync(string remotePath)
    {
        if (!_remoteFileSystem.FileExists(remotePath))
        {
            throw new FileNotFoundException($"Remote file not found: {remotePath}");
        }

        var csvContent = await _remoteFileSystem.ReadAllTextAsync(remotePath);
        
        // Process CSV data (simplified)
        var processedData = csvContent.Replace(",", " | ");
        
        return processedData;
    }

    public async Task BackupDataAsync(string localPath, string remotePath)
    {
        // Ensure remote directory exists
        var remoteDirectory = _remoteFileSystem.GetDirectoryName(remotePath);
        if (!string.IsNullOrEmpty(remoteDirectory) && !_remoteFileSystem.DirectoryExists(remoteDirectory))
        {
            _remoteFileSystem.CreateDirectory(remoteDirectory);
        }

        // Read local file and upload to FTP
        if (File.Exists(localPath))
        {
            var content = await File.ReadAllTextAsync(localPath);
            await _remoteFileSystem.WriteAllTextAsync(remotePath, content);
        }
    }

    public async Task<string[]> ListBackupFilesAsync(string remoteDirectory)
    {
        if (!_remoteFileSystem.DirectoryExists(remoteDirectory))
        {
            return Array.Empty<string>();
        }

        return _remoteFileSystem.GetFiles(remoteDirectory, "*.bak");
    }
}