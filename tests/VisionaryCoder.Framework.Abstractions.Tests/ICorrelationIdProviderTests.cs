// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Reflection;
using FluentAssertions;
using Moq;
using VisionaryCoder.Framework.Abstractions;

namespace VisionaryCoder.Framework.Abstractions.Tests;

[TestClass]
public sealed class ICorrelationIdProviderTests
{
    [TestMethod]
    public void CorrelationId_ShouldReturnCurrentValue()
    {
        // Arrange
        var mockProvider = new Mock<ICorrelationIdProvider>();
        string expectedId = "correlation-12345";
        mockProvider.Setup(p => p.CorrelationId).Returns(expectedId);

        // Act
        string result = mockProvider.Object.CorrelationId;

        // Assert
        result.Should().Be(expectedId);
    }

    [TestMethod]
    public void GenerateNew_ShouldReturnNewCorrelationId()
    {
        // Arrange
        var mockProvider = new Mock<ICorrelationIdProvider>();
        string newId = Guid.NewGuid().ToString();
        mockProvider.Setup(p => p.GenerateNew()).Returns(newId);

        // Act
        string result = mockProvider.Object.GenerateNew();

        // Assert
        result.Should().Be(newId);
        result.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public void SetCorrelationId_ShouldUpdateCurrentValue()
    {
        // Arrange
        var mockProvider = new Mock<ICorrelationIdProvider>();
        string newId = "new-correlation-id";
        mockProvider.Setup(p => p.SetCorrelationId(newId));
        mockProvider.Setup(p => p.CorrelationId).Returns(newId);

        // Act
        mockProvider.Object.SetCorrelationId(newId);
        string result = mockProvider.Object.CorrelationId;

        // Assert
        result.Should().Be(newId);
        mockProvider.Verify(p => p.SetCorrelationId(newId), Times.Once);
    }

    [TestMethod]
    public void GenerateNew_CalledMultipleTimes_ShouldReturnDifferentValues()
    {
        // Arrange
        var mockProvider = new Mock<ICorrelationIdProvider>();
        string id1 = Guid.NewGuid().ToString();
        string id2 = Guid.NewGuid().ToString();
        
        mockProvider.SetupSequence(p => p.GenerateNew())
                    .Returns(id1)
                    .Returns(id2);

        // Act
        string result1 = mockProvider.Object.GenerateNew();
        string result2 = mockProvider.Object.GenerateNew();

        // Assert
        result1.Should().NotBe(result2);
        result1.Should().Be(id1);
        result2.Should().Be(id2);
    }

    [TestMethod]
    public void SetCorrelationId_WithNull_ShouldBeCallable()
    {
        // Arrange
        var mockProvider = new Mock<ICorrelationIdProvider>();
        mockProvider.Setup(p => p.SetCorrelationId(It.IsAny<string>()));

        // Act
        Action act = () => mockProvider.Object.SetCorrelationId(null!);

        // Assert - interface doesn't enforce non-null, implementation should decide
        act.Should().NotThrow();
    }

    [TestMethod]
    public void SetCorrelationId_WithEmptyString_ShouldBeCallable()
    {
        // Arrange
        var mockProvider = new Mock<ICorrelationIdProvider>();
        mockProvider.Setup(p => p.SetCorrelationId(string.Empty));

        // Act
        Action act = () => mockProvider.Object.SetCorrelationId(string.Empty);

        // Assert
        act.Should().NotThrow();
        mockProvider.Verify(p => p.SetCorrelationId(string.Empty), Times.Once);
    }

    [TestMethod]
    public void CorrelationId_AfterGenerateNew_ShouldReflectNewValue()
    {
        // Arrange
        var mockProvider = new Mock<ICorrelationIdProvider>();
        string oldId = "old-id";
        string newId = "new-id";
        
        mockProvider.Setup(p => p.GenerateNew()).Returns(newId).Callback(() => 
        {
            mockProvider.Setup(p => p.CorrelationId).Returns(newId);
        });
        mockProvider.Setup(p => p.CorrelationId).Returns(oldId);

        // Act
        string initialId = mockProvider.Object.CorrelationId;
        mockProvider.Object.GenerateNew();
        string updatedId = mockProvider.Object.CorrelationId;

        // Assert
        initialId.Should().Be(oldId);
        updatedId.Should().Be(newId);
    }

    [TestMethod]
    public void Interface_ShouldHaveCorrectStructure()
    {
        // Arrange & Act
        Type interfaceType = typeof(ICorrelationIdProvider);
        PropertyInfo[] properties = interfaceType.GetProperties();
        MethodInfo[] methods = interfaceType.GetMethods();

        // Assert
        properties.Should().HaveCount(1, "interface has CorrelationId property");
        methods.Should().HaveCount(3, "interface has GenerateNew, SetCorrelationId, and property getter");
    }
}
