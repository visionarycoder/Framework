using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VisionaryCoder.Framework.Services.Abstractions;
using VisionaryCoder.Framework.Services.FileSystem;
using VisionaryCoder.Framework.Secrets.Abstractions;
using VisionaryCoder.Framework.Secrets;

namespace VisionaryCoder.Framework.Examples.SecureFileSystem;

/// <summary>
/// Comprehensive examples demonstrating secure FTP file system usage with secret management.
/// </summary>
public static class SecureFtpFileSystemExamples
{
    /// <summary>
    /// Example 1: Basic secure FTP setup with Azure Key Vault.
    /// This example shows how to configure a secure FTP file system that retrieves credentials from Azure Key Vault.
    /// </summary>
    public static async Task<IHost> Example1_BasicSecureFtpWithKeyVault()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Configure logging
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Debug);
                });

                // Register Azure Key Vault secret provider
                services.AddKeyVaultSecretProvider(options =>
                {
                    options.VaultUri = "https://mykeyvault.vault.azure.net/";
                    options.CacheSecrets = true;
                    options.CacheDuration = TimeSpan.FromMinutes(30);
                });

                // Register secure FTP file system
                services.AddSecureFtpFileSystem(options =>
                {
                    options.Host = "ftp.example.com";
                    options.Port = 21;
                    options.Username = "myftpuser";                    // Direct username
                    options.Password = "secret:ftp-server-password";   // Secret reference
                    options.UseSsl = true;
                    options.CacheCredentials = true;
                    options.CredentialCacheDuration = TimeSpan.FromMinutes(15);
                });
            })
            .Build();

        // Example usage
        var fileSystem = host.Services.GetRequiredService<IFileSystem>();
        
        // Test connection by checking if a directory exists
        var directoryExists = await fileSystem.DirectoryExistsAsync("/uploads");
        Console.WriteLine($"Directory '/uploads' exists: {directoryExists}");
        
        // Upload a test file
        var testContent = "Hello from secure FTP!";
        await fileSystem.WriteAllTextAsync("/uploads/test.txt", testContent);
        
        // Read the file back
        var readContent = await fileSystem.ReadAllTextAsync("/uploads/test.txt");
        Console.WriteLine($"File content: {readContent}");

        return host;
    }

    /// <summary>
    /// Example 2: Multiple secure FTP connections with factory pattern.
    /// This example demonstrates using multiple secure FTP connections for different servers.
    /// </summary>
    public static async Task<IHost> Example2_MultipleSecureFtpConnections()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Configure secret provider
                services.AddKeyVaultSecretProvider(options =>
                {
                    options.VaultUri = "https://mykeyvault.vault.azure.net/";
                    options.CacheSecrets = true;
                });

                // Configure multiple file systems using factory pattern
                services.AddFileSystemFactory()
                    .AddLocal("local")
                    .AddSecureFtp("primary-ftp", new SecureFtpFileSystemOptions
                    {
                        Host = "primary-ftp.example.com",
                        Port = 21,
                        Username = "secret:primary-ftp-username",
                        Password = "secret:primary-ftp-password",
                        UseSsl = true,
                        CacheCredentials = true
                    })
                    .AddSecureFtp("backup-ftp", new SecureFtpFileSystemOptions
                    {
                        Host = "backup-ftp.example.com",
                        Port = 990,
                        Username = "backup-user",
                        Password = "secret:backup-ftp-password",
                        UseSsl = true,
                        UsePassive = true,
                        CacheCredentials = true,
                        CredentialCacheDuration = TimeSpan.FromMinutes(5)
                    });
            })
            .Build();

        // Example usage with factory
        var factory = host.Services.GetRequiredService<IFileSystemFactory>();
        
        // Use different file systems for different purposes
        var primaryFtp = factory.Create("primary-ftp");
        var backupFtp = factory.Create("backup-ftp");
        var local = factory.Create("local");

        // Copy a file from local to primary FTP
        var localContent = await local.ReadAllTextAsync(@"C:\temp\document.txt");
        await primaryFtp.WriteAllTextAsync("/documents/document.txt", localContent);

        // Backup the file to secondary FTP
        await backupFtp.WriteAllTextAsync("/backups/document.txt", localContent);

        Console.WriteLine("File successfully copied to primary and backup FTP servers!");

        return host;
    }

    /// <summary>
    /// Example 3: Secure FTP with comprehensive error handling and monitoring.
    /// This example shows advanced error handling, retry policies, and monitoring.
    /// </summary>
    public static async Task<IHost> Example3_SecureFtpWithMonitoring()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.AddEventLog(); // For Windows event logging
                    builder.SetMinimumLevel(LogLevel.Information);
                });

                // Register secret provider with custom caching
                services.AddKeyVaultSecretProvider(options =>
                {
                    options.VaultUri = "https://production-vault.vault.azure.net/";
                    options.CacheSecrets = true;
                    options.CacheDuration = TimeSpan.FromHours(1);
                });

                // Configure secure FTP with production settings
                services.AddSecureFtpFileSystem(options =>
                {
                    options.Host = "secure-ftp.production.com";
                    options.Port = 990; // Implicit FTPS
                    options.Username = "secret:production-ftp-username";
                    options.Password = "secret:production-ftp-password";
                    options.UseSsl = true;
                    options.UsePassive = true;
                    options.KeepAlive = true;
                    options.TimeoutMilliseconds = 30000; // 30 second timeout
                    options.CacheCredentials = true;
                    options.CredentialCacheDuration = TimeSpan.FromMinutes(30);
                    options.BufferSize = 8192; // 8KB buffer for file transfers
                });
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<SecureFtpFileSystemService>>();
        var fileSystem = host.Services.GetRequiredService<IFileSystem>();

        try
        {
            // Monitor file system operations with detailed logging
            logger.LogInformation("Starting secure FTP file operations monitoring example");

            // Test connectivity with timeout handling
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            
            var connectionTest = await fileSystem.DirectoryExistsAsync("/", cancellationTokenSource.Token);
            logger.LogInformation("FTP connection test result: {ConnectionSuccessful}", connectionTest);

            // Batch file operations with progress tracking
            var filesToProcess = new[] { "file1.txt", "file2.txt", "file3.txt" };
            var successCount = 0;
            var errorCount = 0;

            foreach (var filename in filesToProcess)
            {
                try
                {
                    logger.LogDebug("Processing file: {FileName}", filename);

                    // Simulate file processing
                    var content = $"Processed at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
                    await fileSystem.WriteAllTextAsync($"/processed/{filename}", content, cancellationTokenSource.Token);
                    
                    successCount++;
                    logger.LogInformation("Successfully processed file: {FileName}", filename);
                }
                catch (OperationCanceledException)
                {
                    logger.LogWarning("File processing cancelled: {FileName}", filename);
                    errorCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing file: {FileName}", filename);
                    errorCount++;
                }
            }

            logger.LogInformation("Batch processing completed. Success: {SuccessCount}, Errors: {ErrorCount}", 
                                 successCount, errorCount);

            // Performance monitoring example
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var largeFileContent = new string('A', 1024 * 1024); // 1MB of data
            
            await fileSystem.WriteAllTextAsync("/performance-test/large-file.txt", largeFileContent);
            
            stopwatch.Stop();
            logger.LogInformation("Large file upload completed in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Critical error in secure FTP operations");
            throw;
        }

        return host;
    }

    /// <summary>
    /// Example 4: Unit testing with secure FTP file system mocks.
    /// This example demonstrates how to write testable code using the IFileSystem abstraction.
    /// </summary>
    public class SecureFtpFileProcessorService
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<SecureFtpFileProcessorService> _logger;

        public SecureFtpFileProcessorService(IFileSystem fileSystem, ILogger<SecureFtpFileProcessorService> logger)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Processes files from a secure FTP server with comprehensive error handling.
        /// </summary>
        public async Task<ProcessingResult> ProcessFilesAsync(string remotePath, string pattern = "*.txt", CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Starting file processing from path: {RemotePath}", remotePath);

                if (!await _fileSystem.DirectoryExistsAsync(remotePath, cancellationToken))
                {
                    _logger.LogWarning("Remote directory does not exist: {RemotePath}", remotePath);
                    return new ProcessingResult { Success = false, Message = "Directory not found" };
                }

                var files = await _fileSystem.GetFilesAsync(remotePath, pattern, cancellationToken);
                _logger.LogInformation("Found {FileCount} files to process", files.Length);

                var processedFiles = new List<string>();
                var errors = new List<string>();

                foreach (var file in files)
                {
                    try
                    {
                        var content = await _fileSystem.ReadAllTextAsync(file, cancellationToken);
                        
                        // Process the content (business logic here)
                        var processedContent = ProcessFileContent(content);
                        
                        // Write back processed content
                        var processedPath = file.Replace(".txt", "_processed.txt");
                        await _fileSystem.WriteAllTextAsync(processedPath, processedContent, cancellationToken);
                        
                        processedFiles.Add(file);
                        _logger.LogDebug("Successfully processed file: {File}", file);
                    }
                    catch (Exception ex)
                    {
                        var error = $"Error processing {file}: {ex.Message}";
                        errors.Add(error);
                        _logger.LogError(ex, "Failed to process file: {File}", file);
                    }
                }

                var result = new ProcessingResult
                {
                    Success = errors.Count == 0,
                    ProcessedFiles = processedFiles,
                    Errors = errors,
                    Message = $"Processed {processedFiles.Count} files with {errors.Count} errors"
                };

                _logger.LogInformation("File processing completed: {Message}", result.Message);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error during file processing");
                return new ProcessingResult { Success = false, Message = $"Critical error: {ex.Message}" };
            }
        }

        private string ProcessFileContent(string content)
        {
            // Example business logic: add timestamp and line numbers
            var lines = content.Split('\n');
            var processedLines = lines.Select((line, index) => $"{index + 1:D4}: {line} [Processed: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}]");
            return string.Join('\n', processedLines);
        }
    }

    /// <summary>
    /// Result of file processing operations.
    /// </summary>
    public class ProcessingResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> ProcessedFiles { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }

    /// <summary>
    /// Example 5: Configuration-driven secure FTP setup.
    /// This example shows how to configure secure FTP from configuration files.
    /// </summary>
    public static async Task<IHost> Example5_ConfigurationDrivenSetup()
    {
        // Example appsettings.json structure:
        /*
        {
          "SecureFtp": {
            "Host": "ftp.example.com",
            "Port": 990,
            "Username": "myuser",
            "Password": "secret:my-ftp-password",
            "UseSsl": true,
            "UsePassive": true,
            "CacheCredentials": true,
            "CredentialCacheDurationMinutes": 30,
            "TimeoutSeconds": 30
          },
          "KeyVault": {
            "VaultUri": "https://mykeyvault.vault.azure.net/"
          }
        }
        */

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                // Configure Key Vault from configuration
                services.AddKeyVaultSecretProvider(options =>
                {
                    var keyVaultSection = configuration.GetSection("KeyVault");
                    options.VaultUri = keyVaultSection["VaultUri"] ?? throw new InvalidOperationException("KeyVault:VaultUri is required");
                    options.CacheSecrets = true;
                });

                // Configure secure FTP from configuration
                services.AddSecureFtpFileSystem(options =>
                {
                    var ftpSection = configuration.GetSection("SecureFtp");
                    
                    options.Host = ftpSection["Host"] ?? throw new InvalidOperationException("SecureFtp:Host is required");
                    options.Port = ftpSection.GetValue<int>("Port", 21);
                    options.Username = ftpSection["Username"] ?? throw new InvalidOperationException("SecureFtp:Username is required");
                    options.Password = ftpSection["Password"] ?? throw new InvalidOperationException("SecureFtp:Password is required");
                    options.UseSsl = ftpSection.GetValue<bool>("UseSsl", false);
                    options.UsePassive = ftpSection.GetValue<bool>("UsePassive", true);
                    options.CacheCredentials = ftpSection.GetValue<bool>("CacheCredentials", true);
                    options.CredentialCacheDuration = TimeSpan.FromMinutes(ftpSection.GetValue<int>("CredentialCacheDurationMinutes", 30));
                    options.TimeoutMilliseconds = ftpSection.GetValue<int>("TimeoutSeconds", 30) * 1000;
                });

                // Register the file processor service
                services.AddTransient<SecureFtpFileProcessorService>();
            })
            .Build();

        // Example usage
        var processor = host.Services.GetRequiredService<SecureFtpFileProcessorService>();
        var result = await processor.ProcessFilesAsync("/incoming");
        
        Console.WriteLine($"Processing result: {result.Message}");
        
        if (result.Success)
        {
            Console.WriteLine($"Successfully processed {result.ProcessedFiles.Count} files");
        }
        else
        {
            Console.WriteLine($"Processing failed with {result.Errors.Count} errors");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"  - {error}");
            }
        }

        return host;
    }

    /// <summary>
    /// Example 6: Advanced secret management patterns.
    /// This example demonstrates advanced patterns for managing secrets in secure FTP scenarios.
    /// </summary>
    public static async Task Example6_AdvancedSecretManagement()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Multi-tenant secret management
                services.AddKeyVaultSecretProvider(options =>
                {
                    options.VaultUri = "https://multi-tenant-vault.vault.azure.net/";
                    options.CacheSecrets = true;
                    options.CacheDuration = TimeSpan.FromMinutes(15);
                });

                // Configure secure FTP for different environments
                services.AddFileSystemFactory()
                    .AddSecureFtp("development-ftp", new SecureFtpFileSystemOptions
                    {
                        Host = "dev-ftp.example.com",
                        Username = "secret:dev-ftp-username",
                        Password = "secret:dev-ftp-password",
                        CacheCredentials = true,
                        CredentialCacheDuration = TimeSpan.FromMinutes(5) // Shorter cache for dev
                    })
                    .AddSecureFtp("production-ftp", new SecureFtpFileSystemOptions
                    {
                        Host = "prod-ftp.example.com", 
                        Username = "secret:prod-ftp-username",
                        Password = "secret:prod-ftp-password",
                        CacheCredentials = true,
                        CredentialCacheDuration = TimeSpan.FromHours(1), // Longer cache for prod
                        UseSsl = true
                    });
            })
            .Build();

        var factory = host.Services.GetRequiredService<IFileSystemFactory>();
        
        // Demonstrate environment-specific file operations
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var fileSystemName = environment.ToLower() switch
        {
            "development" => "development-ftp",
            "production" => "production-ftp",
            _ => throw new InvalidOperationException($"Unsupported environment: {environment}")
        };

        var fileSystem = factory.Create(fileSystemName);
        
        // Test the connection with environment-specific settings
        var testResult = await fileSystem.FileExistsAsync("/health-check.txt");
        Console.WriteLine($"Health check for {environment} environment: {testResult}");
        
        // Demonstrate credential cache management for different environments
        if (fileSystem is SecureFtpFileSystemService secureFtp)
        {
            // Clear credential cache if needed (useful for credential rotation scenarios)
            secureFtp.ClearCredentialCache();
            Console.WriteLine($"Cleared credential cache for {environment} environment");
        }

        await host.StopAsync();
    }
}

/// <summary>
/// Unit test examples for secure FTP file system.
/// </summary>
public static class SecureFtpFileSystemTests
{
    /// <summary>
    /// Example unit test using Moq to mock the IFileSystem interface.
    /// </summary>
    public static async Task ExampleUnitTest_FileProcessorWithMocks()
    {
        // Arrange
        var mockFileSystem = new Mock<IFileSystem>();
        var mockLogger = new Mock<ILogger<SecureFtpFileSystemExamples.SecureFtpFileProcessorService>>();

        // Setup mock behavior
        mockFileSystem.Setup(fs => fs.DirectoryExistsAsync("/test", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

        mockFileSystem.Setup(fs => fs.GetFilesAsync("/test", "*.txt", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new[] { "/test/file1.txt", "/test/file2.txt" });

        mockFileSystem.Setup(fs => fs.ReadAllTextAsync("/test/file1.txt", It.IsAny<CancellationToken>()))
                     .ReturnsAsync("Test content 1");

        mockFileSystem.Setup(fs => fs.ReadAllTextAsync("/test/file2.txt", It.IsAny<CancellationToken>()))
                     .ReturnsAsync("Test content 2");

        mockFileSystem.Setup(fs => fs.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

        // Act
        var processor = new SecureFtpFileSystemExamples.SecureFtpFileProcessorService(mockFileSystem.Object, mockLogger.Object);
        var result = await processor.ProcessFilesAsync("/test");

        // Assert
        Console.WriteLine($"Test result: Success = {result.Success}, Message = {result.Message}");
        Console.WriteLine($"Processed files: {result.ProcessedFiles.Count}");
        
        // Verify interactions
        mockFileSystem.Verify(fs => fs.DirectoryExistsAsync("/test", It.IsAny<CancellationToken>()), Times.Once);
        mockFileSystem.Verify(fs => fs.GetFilesAsync("/test", "*.txt", It.IsAny<CancellationToken>()), Times.Once);
        mockFileSystem.Verify(fs => fs.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}