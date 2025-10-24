// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using FluentAssertions;
using Moq;
using VisionaryCoder.Framework.Abstractions;

namespace VisionaryCoder.Framework.Abstractions.Tests;

[TestClass]
public sealed class IFrameworkInfoProviderTests
{
    [TestMethod]
    public void Version_ShouldReturnFrameworkVersion()
    {
        // Arrange
        var mockProvider = new Mock<IFrameworkInfoProvider>();
        var version = "1.0.0";
        mockProvider.Setup(p => p.Version).Returns(version);

        // Act
        var result = mockProvider.Object.Version;

        // Assert
        result.Should().Be(version);
    }

    [TestMethod]
    public void Name_ShouldReturnFrameworkName()
    {
        // Arrange
        var mockProvider = new Mock<IFrameworkInfoProvider>();
        var name = "VisionaryCoder.Framework";
        mockProvider.Setup(p => p.Name).Returns(name);

        // Act
        var result = mockProvider.Object.Name;

        // Assert
        result.Should().Be(name);
    }

    [TestMethod]
    public void Description_ShouldReturnFrameworkDescription()
    {
        // Arrange
        var mockProvider = new Mock<IFrameworkInfoProvider>();
        var description = "Enterprise framework for .NET applications";
        mockProvider.Setup(p => p.Description).Returns(description);

        // Act
        var result = mockProvider.Object.Description;

        // Assert
        result.Should().Be(description);
    }

    [TestMethod]
    public void CompiledAt_ShouldReturnCompilationTimestamp()
    {
        // Arrange
        var mockProvider = new Mock<IFrameworkInfoProvider>();
        var compiledAt = DateTimeOffset.UtcNow;
        mockProvider.Setup(p => p.CompiledAt).Returns(compiledAt);

        // Act
        var result = mockProvider.Object.CompiledAt;

        // Assert
        result.Should().Be(compiledAt);
    }

    [TestMethod]
    public void CompiledAt_ShouldBeInPast()
    {
        // Arrange
        var mockProvider = new Mock<IFrameworkInfoProvider>();
        var pastDate = DateTimeOffset.UtcNow.AddDays(-1);
        mockProvider.Setup(p => p.CompiledAt).Returns(pastDate);

        // Act
        var result = mockProvider.Object.CompiledAt;

        // Assert
        result.Should().BeBefore(DateTimeOffset.UtcNow);
    }

    [TestMethod]
    public void AllProperties_ShouldBeReadable()
    {
        // Arrange
        var mockProvider = new Mock<IFrameworkInfoProvider>();
        mockProvider.Setup(p => p.Version).Returns("2.0.0");
        mockProvider.Setup(p => p.Name).Returns("TestFramework");
        mockProvider.Setup(p => p.Description).Returns("Test Description");
        mockProvider.Setup(p => p.CompiledAt).Returns(DateTimeOffset.UtcNow);

        // Act
        var version = mockProvider.Object.Version;
        var name = mockProvider.Object.Name;
        var description = mockProvider.Object.Description;
        var compiledAt = mockProvider.Object.CompiledAt;

        // Assert
        version.Should().NotBeNullOrWhiteSpace();
        name.Should().NotBeNullOrWhiteSpace();
        description.Should().NotBeNullOrWhiteSpace();
        compiledAt.Should().NotBe(default);
    }

    [TestMethod]
    public void Interface_ShouldHaveFourProperties()
    {
        // Arrange & Act
        var interfaceType = typeof(IFrameworkInfoProvider);
        var properties = interfaceType.GetProperties();

        // Assert
        properties.Should().HaveCount(4, "interface has Version, Name, Description, and CompiledAt properties");
    }

    [TestMethod]
    public void Version_ShouldFollowSemanticVersioning()
    {
        // Arrange
        var mockProvider = new Mock<IFrameworkInfoProvider>();
        var version = "1.2.3";
        mockProvider.Setup(p => p.Version).Returns(version);

        // Act
        var result = mockProvider.Object.Version;

        // Assert
        result.Should().MatchRegex(@"^\d+\.\d+\.\d+", "version should follow semantic versioning pattern");
    }

    [TestMethod]
    public void CompiledAt_WithUtcTime_ShouldHaveZeroOffset()
    {
        // Arrange
        var mockProvider = new Mock<IFrameworkInfoProvider>();
        var utcTime = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        mockProvider.Setup(p => p.CompiledAt).Returns(utcTime);

        // Act
        var result = mockProvider.Object.CompiledAt;

        // Assert
        result.Offset.Should().Be(TimeSpan.Zero);
    }

    [TestMethod]
    public void Name_ShouldContainVisionaryCoder()
    {
        // Arrange
        var mockProvider = new Mock<IFrameworkInfoProvider>();
        var name = "VisionaryCoder.Framework";
        mockProvider.Setup(p => p.Name).Returns(name);

        // Act
        var result = mockProvider.Object.Name;

        // Assert
        result.Should().Contain("VisionaryCoder");
    }
}
