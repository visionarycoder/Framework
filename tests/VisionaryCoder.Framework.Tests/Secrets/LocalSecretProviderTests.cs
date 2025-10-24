using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using VisionaryCoder.Framework.Configuration.Azure;
using VisionaryCoder.Framework.Configuration.Secrets;

namespace VisionaryCoder.Framework.Tests.Secrets;

[TestClass]
public class LocalSecretProviderTests
{
    private Mock<IConfiguration> mockConfiguration = null!;
    private KeyVaultOptions options = null!;

    [TestInitialize]
    public void Setup()
    {
        mockConfiguration = new Mock<IConfiguration>();
        options = new KeyVaultOptions { LocalSecretsPrefix = "Secrets" };
    }

    [TestMethod]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Act
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);

        // Assert
        provider.Should().NotBeNull();
    }

    [TestMethod]
    public void Constructor_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new LocalSecretProvider(null!, options);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("configuration");
    }

    [TestMethod]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new LocalSecretProvider(mockConfiguration.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("options");
    }

    [TestMethod]
    public async Task GetAsync_WithNullName_ShouldThrowArgumentException()
    {
        // Arrange
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);

        // Act
        Func<Task> act = async () => await provider.GetAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Secret name cannot be null or empty*")
            .WithParameterName("name");
    }

    [TestMethod]
    public async Task GetAsync_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);

        // Act
        Func<Task> act = async () => await provider.GetAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Secret name cannot be null or empty*");
    }

    [TestMethod]
    [DataRow(" ")]
    [DataRow("  ")]
    public async Task GetAsync_WithWhitespaceName_ShouldReturnNull(string secretName)
    {
        // Arrange
        mockConfiguration.SetupGet(c => c[It.IsAny<string>()]).Returns((string?)null);
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);

        // Act
        var result = await provider.GetAsync(secretName);

        // Assert
        result.Should().BeNull("whitespace-only names are treated as valid but non-existent secrets");
    }

    [TestMethod]
    public async Task GetAsync_WithPrefixedKeyInConfiguration_ShouldReturnValue()
    {
        // Arrange
        var secretName = "ApiKey";
        var expectedValue = "test-api-key-value";
        mockConfiguration.SetupGet(c => c["Secrets:ApiKey"]).Returns(expectedValue);
        
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);

        // Act
        var result = await provider.GetAsync(secretName);

        // Assert
        result.Should().Be(expectedValue);
    }

    [TestMethod]
    public async Task GetAsync_WithDirectKeyInConfiguration_ShouldReturnValue()
    {
        // Arrange
        var secretName = "DatabasePassword";
        var expectedValue = "direct-password";
        mockConfiguration.SetupGet(c => c["Secrets:DatabasePassword"]).Returns((string?)null);
        mockConfiguration.SetupGet(c => c["DatabasePassword"]).Returns(expectedValue);
        
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);

        // Act
        var result = await provider.GetAsync(secretName);

        // Assert
        result.Should().Be(expectedValue);
    }

    [TestMethod]
    public async Task GetAsync_WithEnvironmentVariable_ShouldReturnValue()
    {
        // Arrange
        var secretName = "TEST_ENV_SECRET";
        var expectedValue = "env-secret-value";
        Environment.SetEnvironmentVariable(secretName, expectedValue);
        
        mockConfiguration.SetupGet(c => c[$"Secrets:{secretName}"]).Returns((string?)null);
        mockConfiguration.SetupGet(c => c[secretName]).Returns((string?)null);
        
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);

        try
        {
            // Act
            var result = await provider.GetAsync(secretName);

            // Assert
            result.Should().Be(expectedValue);
        }
        finally
        {
            Environment.SetEnvironmentVariable(secretName, null);
        }
    }

    [TestMethod]
    public async Task GetAsync_PrefixedKeyTakesPriority_OverDirectKey()
    {
        // Arrange
        var secretName = "ConnectionString";
        var prefixedValue = "prefixed-connection-string";
        var directValue = "direct-connection-string";
        
        mockConfiguration.SetupGet(c => c["Secrets:ConnectionString"]).Returns(prefixedValue);
        mockConfiguration.SetupGet(c => c["ConnectionString"]).Returns(directValue);
        
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);

        // Act
        var result = await provider.GetAsync(secretName);

        // Assert
        result.Should().Be(prefixedValue, "prefixed key should take priority");
    }

    [TestMethod]
    public async Task GetAsync_DirectKeyTakesPriority_OverEnvironmentVariable()
    {
        // Arrange
        var secretName = "TEST_PRIORITY_SECRET";
        var configValue = "config-value";
        var envValue = "env-value";
        
        Environment.SetEnvironmentVariable(secretName, envValue);
        mockConfiguration.SetupGet(c => c[$"Secrets:{secretName}"]).Returns((string?)null);
        mockConfiguration.SetupGet(c => c[secretName]).Returns(configValue);
        
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);

        try
        {
            // Act
            var result = await provider.GetAsync(secretName);

            // Assert
            result.Should().Be(configValue, "configuration should take priority over environment variable");
        }
        finally
        {
            Environment.SetEnvironmentVariable(secretName, null);
        }
    }

    [TestMethod]
    public async Task GetAsync_WithNonExistentSecret_ShouldReturnNull()
    {
        // Arrange
        var secretName = "NonExistentSecret";
        mockConfiguration.SetupGet(c => c[It.IsAny<string>()]).Returns((string?)null);
        
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);

        // Act
        var result = await provider.GetAsync(secretName);

        // Assert
        result.Should().BeNull("non-existent secrets should return null");
    }

    [TestMethod]
    [DataRow("ApiKey", "api-key-123")]
    [DataRow("DbPassword", "password-456")]
    [DataRow("ServiceToken", "token-789")]
    public async Task GetAsync_WithVariousSecretNames_ShouldCheckPrefixedKey(string secretName, string expectedValue)
    {
        // Arrange
        mockConfiguration.SetupGet(c => c[$"Secrets:{secretName}"]).Returns(expectedValue);
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);

        // Act
        var result = await provider.GetAsync(secretName);

        // Assert
        result.Should().Be(expectedValue);
    }

    [TestMethod]
    public async Task GetAsync_WithCancellationToken_ShouldNotThrow()
    {
        // Arrange
        mockConfiguration.SetupGet(c => c["Secrets:TestSecret"]).Returns("test-value");
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);
        using var cts = new CancellationTokenSource();

        // Act
        Func<Task> act = async () => await provider.GetAsync("TestSecret", cts.Token);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [TestMethod]
    public async Task GetAsync_WithCanceledToken_ShouldNotCheckCancellation()
    {
        // Arrange
        mockConfiguration.SetupGet(c => c["Secrets:TestSecret"]).Returns("test-value");
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var result = await provider.GetAsync("TestSecret", cts.Token);

        // Assert
        result.Should().Be("test-value", "LocalSecretProvider doesn't check cancellation token");
    }

    [TestMethod]
    public async Task GetAsync_CalledMultipleTimes_ShouldCheckConfigurationEachTime()
    {
        // Arrange
        var secretName = "ApiKey";
        var value1 = "value-1";
        var value2 = "value-2";
        
        var setupSequence = mockConfiguration.SetupSequence(c => c[$"Secrets:{secretName}"])
            .Returns(value1)
            .Returns(value2);
        
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);

        // Act
        var result1 = await provider.GetAsync(secretName);
        var result2 = await provider.GetAsync(secretName);

        // Assert
        result1.Should().Be(value1);
        result2.Should().Be(value2);
    }

    [TestMethod]
    public async Task GetAsync_WithCustomPrefix_ShouldUseCustomPrefix()
    {
        // Arrange
        var customOptions = new KeyVaultOptions { LocalSecretsPrefix = "CustomSecrets" };
        var secretName = "ApiKey";
        var expectedValue = "custom-api-key";
        
        mockConfiguration.SetupGet(c => c["CustomSecrets:ApiKey"]).Returns(expectedValue);
        var provider = new LocalSecretProvider(mockConfiguration.Object, customOptions);

        // Act
        var result = await provider.GetAsync(secretName);

        // Assert
        result.Should().Be(expectedValue);
    }

    [TestMethod]
    public void LocalSecretProvider_ShouldImplementISecretProvider()
    {
        // Arrange & Act
        var provider = new LocalSecretProvider(mockConfiguration.Object, options);

        // Assert
        provider.Should().BeAssignableTo<VisionaryCoder.Framework.Abstractions.Services.ISecretProvider>();
    }
}
