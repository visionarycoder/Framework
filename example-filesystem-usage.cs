using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Services.Abstractions;
using VisionaryCoder.Framework.Services.FileSystem;

namespace VisionaryCoder.Framework.Examples;

/// <summary>
/// Example demonstrating how to configure and use different file system implementations.
/// Shows both local file system and FTP file system configurations following Microsoft patterns.
/// </summary>
public class FileSystemUsageExamples
{
    /// <summary>
    /// Example of configuring local file system services in a typical ASP.NET Core application.
    /// </summary>
    public static void ConfigureLocalFileSystemServices(IServiceCollection services)
    {
        // Add logging (required for file system services)
        services.AddLogging(builder => builder.AddConsole());

        // Register local file system services
        services.AddFileSystemServices();

        // Alternative: Register as singleton if you want to share instance
        // services.AddFileSystemServicesSingleton();
    }

    /// <summary>
    /// Example of configuring FTP file system services with explicit options.
    /// </summary>
    public static void ConfigureFtpFileSystemServices(IServiceCollection services)
    {
        // Add logging (required for file system services)
        services.AddLogging(builder => builder.AddConsole());

        // Configure FTP options
        var ftpOptions = new FtpFileSystemOptions
        {
            Host = "ftp.example.com",
            Port = 21,
            Username = "myuser",
            Password = "mypassword",
            UseSsl = false,              // Set to true for FTPS
            UsePassive = true,           // Recommended for most firewalls
            TimeoutMilliseconds = 30000, // 30 seconds
            UseBinary = true,            // Recommended for most file types
            BufferSize = 8192           // 8KB buffer for transfers
        };

        // Register FTP file system services
        services.AddFtpFileSystemServices(ftpOptions);
    }

    /// <summary>
    /// Example of configuring secure FTPS file system services.
    /// </summary>
    public static void ConfigureSecureFtpFileSystemServices(IServiceCollection services)
    {
        // Add logging
        services.AddLogging(builder => builder.AddConsole());

        // Configure secure FTP (FTPS) options
        var ftpsOptions = new FtpFileSystemOptions
        {
            Host = "secure.ftp.example.com",
            Port = 990,                  // Standard FTPS port
            Username = "secureuser",
            Password = "securepassword",
            UseSsl = true,              // Enable SSL/TLS encryption
            UsePassive = true,
            TimeoutMilliseconds = 60000, // Longer timeout for encrypted connections
            UseBinary = true,
            BufferSize = 16384          // Larger buffer for encrypted transfers
        };

        // Register as singleton for better connection reuse
        services.AddFtpFileSystemServicesSingleton(ftpsOptions);
    }

    /// <summary>
    /// Example of configuring multiple named FTP services for different servers.
    /// </summary>
    public static void ConfigureMultipleFtpServices(IServiceCollection services)
    {
        // Add logging
        services.AddLogging(builder => builder.AddConsole());

        // Primary FTP server for uploads
        var uploadFtpOptions = new FtpFileSystemOptions
        {
            Host = "uploads.ftp.example.com",
            Username = "uploaduser",
            Password = "uploadpass"
        };

        // Backup FTP server for archives
        var backupFtpOptions = new FtpFileSystemOptions
        {
            Host = "backup.ftp.example.com",
            Username = "backupuser",
            Password = "backuppass",
            UseSsl = true,
            Port = 990
        };

        // Register named services
        services.AddNamedFtpFileSystemServices("uploads", uploadFtpOptions);
        services.AddNamedFtpFileSystemServices("backup", backupFtpOptions);
    }

    /// <summary>
    /// Example service demonstrating how to use IFileSystem in an accessor component.
    /// This follows VBD (Volatility-Based Decomposition) architecture patterns.
    /// </summary>
    public class DocumentAccessor
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<DocumentAccessor> _logger;

        public DocumentAccessor(IFileSystem fileSystem, ILogger<DocumentAccessor> logger)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Saves a document to the configured file system (local or remote).
        /// </summary>
        public async Task SaveDocumentAsync(string path, string content, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Saving document to {Path}", path);

                // Ensure directory exists
                var directory = _fileSystem.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !_fileSystem.DirectoryExists(directory))
                {
                    _fileSystem.CreateDirectory(directory);
                    _logger.LogDebug("Created directory {Directory}", directory);
                }

                // Save the document
                await _fileSystem.WriteAllTextAsync(path, content, cancellationToken);
                
                _logger.LogInformation("Successfully saved document to {Path}", path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save document to {Path}", path);
                throw;
            }
        }

        /// <summary>
        /// Loads a document from the configured file system.
        /// </summary>
        public async Task<string?> LoadDocumentAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Loading document from {Path}", path);

                if (!_fileSystem.FileExists(path))
                {
                    _logger.LogWarning("Document not found at {Path}", path);
                    return null;
                }

                var content = await _fileSystem.ReadAllTextAsync(path, cancellationToken);
                _logger.LogInformation("Successfully loaded document from {Path} ({Length} characters)", 
                                     path, content.Length);
                
                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load document from {Path}", path);
                throw;
            }
        }

        /// <summary>
        /// Lists all documents matching a pattern in the specified directory.
        /// </summary>
        public async Task<string[]> ListDocumentsAsync(string directoryPath, string pattern = "*.txt")
        {
            try
            {
                _logger.LogInformation("Listing documents in {Directory} with pattern {Pattern}", directoryPath, pattern);

                if (!_fileSystem.DirectoryExists(directoryPath))
                {
                    _logger.LogWarning("Directory not found: {Directory}", directoryPath);
                    return Array.Empty<string>();
                }

                var files = _fileSystem.GetFiles(directoryPath, pattern);
                _logger.LogInformation("Found {Count} documents in {Directory}", files.Length, directoryPath);
                
                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list documents in {Directory}", directoryPath);
                throw;
            }
        }

        /// <summary>
        /// Archives old documents by moving them to a backup location.
        /// </summary>
        public async Task ArchiveDocumentsAsync(string sourceDirectory, string archiveDirectory, 
                                              DateTime cutoffDate, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Archiving documents from {Source} to {Archive} (cutoff: {Cutoff})", 
                                     sourceDirectory, archiveDirectory, cutoffDate);

                // Ensure archive directory exists
                if (!_fileSystem.DirectoryExists(archiveDirectory))
                {
                    _fileSystem.CreateDirectory(archiveDirectory);
                }

                // Get all files in source directory
                var files = _fileSystem.GetFiles(sourceDirectory);
                var archivedCount = 0;

                foreach (var filePath in files)
                {
                    // For demonstration, we'll use a simple naming convention to determine file date
                    // In real scenarios, you'd check file metadata or parse filenames
                    var fileName = _fileSystem.GetFileName(filePath);
                    var archivePath = $"{archiveDirectory.TrimEnd('/')}/{fileName}";

                    // Read and write file (simulating move operation)
                    var content = await _fileSystem.ReadAllTextAsync(filePath, cancellationToken);
                    await _fileSystem.WriteAllTextAsync(archivePath, content, cancellationToken);
                    _fileSystem.DeleteFile(filePath);

                    archivedCount++;
                    _logger.LogDebug("Archived {Source} to {Archive}", filePath, archivePath);
                }

                _logger.LogInformation("Successfully archived {Count} documents", archivedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to archive documents from {Source} to {Archive}", 
                               sourceDirectory, archiveDirectory);
                throw;
            }
        }
    }
}

/// <summary>
/// Example of a console application using the file system services.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        // Build the host
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Configure file system services based on environment or configuration
                var useLocalFileSystem = context.Configuration.GetValue<bool>("UseLocalFileSystem", true);
                
                if (useLocalFileSystem)
                {
                    FileSystemUsageExamples.ConfigureLocalFileSystemServices(services);
                }
                else
                {
                    FileSystemUsageExamples.ConfigureFtpFileSystemServices(services);
                }

                // Register your accessor service
                services.AddScoped<FileSystemUsageExamples.DocumentAccessor>();
            })
            .Build();

        // Use the services
        using var scope = host.Services.CreateScope();
        var documentAccessor = scope.ServiceProvider.GetRequiredService<FileSystemUsageExamples.DocumentAccessor>();
        
        try
        {
            // Example usage
            await documentAccessor.SaveDocumentAsync("/documents/example.txt", "Hello, World!");
            var content = await documentAccessor.LoadDocumentAsync("/documents/example.txt");
            Console.WriteLine($"Loaded content: {content}");

            var documents = await documentAccessor.ListDocumentsAsync("/documents");
            Console.WriteLine($"Found {documents.Length} documents");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

/// <summary>
/// Example appsettings.json configuration for different environments.
/// </summary>
public class ExampleConfiguration
{
    /*
    // appsettings.json
    {
      "UseLocalFileSystem": true,
      "Ftp": {
        "Host": "ftp.example.com",
        "Port": 21,
        "Username": "user",
        "Password": "pass",
        "UseSsl": false,
        "UsePassive": true,
        "TimeoutMilliseconds": 30000,
        "UseBinary": true,
        "BufferSize": 8192
      }
    }

    // appsettings.Production.json
    {
      "UseLocalFileSystem": false,
      "Ftp": {
        "Host": "prod.ftp.company.com",
        "Port": 990,
        "Username": "prod_user",
        "Password": "secure_password",
        "UseSsl": true,
        "UsePassive": true,
        "TimeoutMilliseconds": 60000,
        "UseBinary": true,
        "BufferSize": 16384
      }
    }
    */
}