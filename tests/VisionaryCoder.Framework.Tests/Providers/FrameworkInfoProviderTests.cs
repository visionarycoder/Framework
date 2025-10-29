using FluentAssertions;
using VisionaryCoder.Framework;
using VisionaryCoder.Framework.Providers;

namespace VisionaryCoder.Framework.Tests.Providers;

[TestClass]
public class FrameworkInfoProviderTests
{
    [TestMethod]
    public void Name_ShouldReturnFrameworkName()
    {
        // Arrange
        var provider = new FrameworkInfoProvider();

        // Act
        var name = provider.Name;

        // Assert
        name.Should().Be("VisionaryCoder Framework");
    }

    [TestMethod]
    public void Name_ShouldNotBeNullOrEmpty()
    {
        // Arrange
        var provider = new FrameworkInfoProvider();

        // Act
        var name = provider.Name;

        // Assert
        name.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public void Description_ShouldReturnFrameworkDescription()
    {
        // Arrange
        var provider = new FrameworkInfoProvider();

        // Act
        var description = provider.Description;

        // Assert
        description.Should().Be("A comprehensive framework for building enterprise-grade applications with proxy interceptor architecture.");
    }

    [TestMethod]
    public void Description_ShouldNotBeNullOrEmpty()
    {
        // Arrange
        var provider = new FrameworkInfoProvider();

        // Act
        var description = provider.Description;

        // Assert
        description.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public void Version_ShouldNotBeNullOrEmpty()
    {
        // Arrange
        var provider = new FrameworkInfoProvider();

        // Act
        var version = provider.Version;

        // Assert
        version.Should().NotBeNullOrEmpty("version should always be available");
    }

    [TestMethod]
    [DataRow("0.0.0")]
    [DataRow("1.0.0")]
    [DataRow("2.3.4")]
    public void Version_ShouldMatchSemanticVersioningPattern(string expectedPattern)
    {
        // Arrange
        var provider = new FrameworkInfoProvider();

        // Act
        var version = provider.Version;

        // Assert
        version.Should().MatchRegex(@"^\d+\.\d+\.\d+",
            $"version should follow semantic versioning (e.g., {expectedPattern})");
    }

    [TestMethod]
    public void Version_ShouldBeConsistentAcrossMultipleCalls()
    {
        // Arrange
        var provider = new FrameworkInfoProvider();

        // Act
        var version1 = provider.Version;
        var version2 = provider.Version;
        var version3 = provider.Version;

        // Assert
        version1.Should().Be(version2);
        version2.Should().Be(version3);
    }

    [TestMethod]
    public void CompiledAt_ShouldBeInThePast()
    {
        // Arrange
        var provider = new FrameworkInfoProvider();
        DateTimeOffset now = DateTimeOffset.UtcNow;

        // Act
        var compiledAt = provider.CompiledAt;

        // Assert
        compiledAt.Should().BeBefore(now, "compiled time must be in the past");
    }

    [TestMethod]
    public void CompiledAt_ShouldBeReasonablyRecent()
    {
        // Arrange
        var provider = new FrameworkInfoProvider();
        DateTimeOffset oneYearAgo = DateTimeOffset.UtcNow.AddYears(-1);

        // Act
        var compiledAt = provider.CompiledAt;

        // Assert
        compiledAt.Should().BeAfter(oneYearAgo, "compiled time should be within the last year");
    }

    [TestMethod]
    public void CompiledAt_ShouldBeConsistentAcrossMultipleCalls()
    {
        // Arrange
        var provider = new FrameworkInfoProvider();

        // Act
        var time1 = provider.CompiledAt;
        var time2 = provider.CompiledAt;
        var time3 = provider.CompiledAt;

        // Assert
        time1.Should().Be(time2);
        time2.Should().Be(time3);
    }

    [TestMethod]
    public void FrameworkInfoProvider_MultiplInstances_ShouldReturnSameValues()
    {
        // Arrange
        var provider1 = new FrameworkInfoProvider();
        var provider2 = new FrameworkInfoProvider();

        // Act & Assert
        provider1.Name.Should().Be(provider2.Name);
        provider1.Description.Should().Be(provider2.Description);
        provider1.Version.Should().Be(provider2.Version);
        provider1.CompiledAt.Should().Be(provider2.CompiledAt, "compiled time is determined at assembly load");
    }

    [TestMethod]
    public void FrameworkInfoProvider_AllProperties_ShouldBeAccessibleWithoutException()
    {
        // Arrange
        var provider = new FrameworkInfoProvider();

        // Act & Assert
        var name = () => provider.Name;
        var description = () => provider.Description;
        var version = () => provider.Version;
        var compiledAt = () => provider.CompiledAt;

        name.Should().NotThrow();
        description.Should().NotThrow();
        version.Should().NotThrow();
        compiledAt.Should().NotThrow();
    }

    [TestMethod]
    public void CompiledAt_Year_ShouldBeReasonable()
    {
        // Arrange
        var provider = new FrameworkInfoProvider();
        int[] reasonableYears = new[] { 2024, 2025, 2026, 2027 };

        // Act
        var compiledAt = provider.CompiledAt;

        // Assert
        compiledAt.Year.Should().BeOneOf(reasonableYears,
            "compiled year should be recent (test created in 2025)");
    }
}
