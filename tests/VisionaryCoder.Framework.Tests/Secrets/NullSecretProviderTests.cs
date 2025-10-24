using FluentAssertions;
using VisionaryCoder.Framework.Configuration.Secrets;

namespace VisionaryCoder.Framework.Tests.Secrets;

[TestClass]
public class NullSecretProviderTests
{
    [TestMethod]
    public void Instance_ShouldReturnSameInstanceEveryTime()
    {
        // Act
        var instance1 = NullSecretProvider.Instance;
        var instance2 = NullSecretProvider.Instance;
        var instance3 = NullSecretProvider.Instance;

        // Assert
        instance1.Should().BeSameAs(instance2, "Instance should be a singleton");
        instance2.Should().BeSameAs(instance3, "Instance should be a singleton");
    }

    [TestMethod]
    public void Instance_ShouldNotBeNull()
    {
        // Act
        var instance = NullSecretProvider.Instance;

        // Assert
        instance.Should().NotBeNull("singleton instance should always be available");
    }

    [TestMethod]
    public async Task GetAsync_WithAnyName_ShouldReturnNull()
    {
        // Arrange
        var provider = NullSecretProvider.Instance;

        // Act
        var result = await provider.GetAsync("any-secret-name");

        // Assert
        result.Should().BeNull("NullSecretProvider always returns null");
    }

    [TestMethod]
    [DataRow("ApiKey")]
    [DataRow("ConnectionString")]
    [DataRow("DatabasePassword")]
    [DataRow("")]
    [DataRow(" ")]
    public async Task GetAsync_WithVariousNames_ShouldAlwaysReturnNull(string secretName)
    {
        // Arrange
        var provider = NullSecretProvider.Instance;

        // Act
        var result = await provider.GetAsync(secretName);

        // Assert
        result.Should().BeNull($"NullSecretProvider should return null for '{secretName}'");
    }

    [TestMethod]
    public async Task GetAsync_WithCancellationToken_ShouldStillReturnNull()
    {
        // Arrange
        var provider = NullSecretProvider.Instance;
        using var cts = new CancellationTokenSource();

        // Act
        var result = await provider.GetAsync("secret-name", cts.Token);

        // Assert
        result.Should().BeNull("NullSecretProvider returns null regardless of cancellation token");
    }

    [TestMethod]
    public async Task GetAsync_WithCanceledToken_ShouldNotThrow()
    {
        // Arrange
        var provider = NullSecretProvider.Instance;
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await provider.GetAsync("secret-name", cts.Token);

        // Assert
        await act.Should().NotThrowAsync("NullSecretProvider doesn't check cancellation");
    }

    [TestMethod]
    public async Task GetAsync_CalledMultipleTimes_ShouldAlwaysReturnNull()
    {
        // Arrange
        var provider = NullSecretProvider.Instance;
        var secretName = "test-secret";

        // Act
        var result1 = await provider.GetAsync(secretName);
        var result2 = await provider.GetAsync(secretName);
        var result3 = await provider.GetAsync(secretName);

        // Assert
        result1.Should().BeNull();
        result2.Should().BeNull();
        result3.Should().BeNull();
    }

    [TestMethod]
    public async Task GetAsync_MultipleConcurrentCalls_ShouldAllReturnNull()
    {
        // Arrange
        var provider = NullSecretProvider.Instance;
        var tasks = new List<Task<string?>>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(provider.GetAsync($"secret-{i}"));
        }
        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().AllBe(null, "all results should be null");
    }

    [TestMethod]
    public async Task GetAsync_WithNullName_ShouldReturnNull()
    {
        // Arrange
        var provider = NullSecretProvider.Instance;

        // Act
        var result = await provider.GetAsync(null!);

        // Assert
        result.Should().BeNull("NullSecretProvider doesn't validate input");
    }

    [TestMethod]
    public void NullSecretProvider_ShouldImplementISecretProvider()
    {
        // Arrange
        var provider = NullSecretProvider.Instance;

        // Assert
        provider.Should().BeAssignableTo<VisionaryCoder.Framework.Abstractions.Services.ISecretProvider>();
    }
}
