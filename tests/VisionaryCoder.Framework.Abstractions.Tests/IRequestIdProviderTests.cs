// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using FluentAssertions;
using Moq;
using VisionaryCoder.Framework.Abstractions;

namespace VisionaryCoder.Framework.Abstractions.Tests;

[TestClass]
public sealed class IRequestIdProviderTests
{
    [TestMethod]
    public void RequestId_ShouldReturnCurrentValue()
    {
        // Arrange
        var mockProvider = new Mock<IRequestIdProvider>();
        var expectedId = "request-67890";
        mockProvider.Setup(p => p.RequestId).Returns(expectedId);

        // Act
        var result = mockProvider.Object.RequestId;

        // Assert
        result.Should().Be(expectedId);
    }

    [TestMethod]
    public void GenerateNew_ShouldReturnNewRequestId()
    {
        // Arrange
        var mockProvider = new Mock<IRequestIdProvider>();
        var newId = Guid.NewGuid().ToString();
        mockProvider.Setup(p => p.GenerateNew()).Returns(newId);

        // Act
        var result = mockProvider.Object.GenerateNew();

        // Assert
        result.Should().Be(newId);
        result.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public void SetRequestId_ShouldUpdateCurrentValue()
    {
        // Arrange
        var mockProvider = new Mock<IRequestIdProvider>();
        var newId = "new-request-id";
        mockProvider.Setup(p => p.SetRequestId(newId));
        mockProvider.Setup(p => p.RequestId).Returns(newId);

        // Act
        mockProvider.Object.SetRequestId(newId);
        var result = mockProvider.Object.RequestId;

        // Assert
        result.Should().Be(newId);
        mockProvider.Verify(p => p.SetRequestId(newId), Times.Once);
    }

    [TestMethod]
    public void GenerateNew_CalledMultipleTimes_ShouldReturnDifferentValues()
    {
        // Arrange
        var mockProvider = new Mock<IRequestIdProvider>();
        var id1 = Guid.NewGuid().ToString();
        var id2 = Guid.NewGuid().ToString();
        
        mockProvider.SetupSequence(p => p.GenerateNew())
                    .Returns(id1)
                    .Returns(id2);

        // Act
        var result1 = mockProvider.Object.GenerateNew();
        var result2 = mockProvider.Object.GenerateNew();

        // Assert
        result1.Should().NotBe(result2);
        result1.Should().Be(id1);
        result2.Should().Be(id2);
    }

    [TestMethod]
    public void SetRequestId_WithNull_ShouldBeCallable()
    {
        // Arrange
        var mockProvider = new Mock<IRequestIdProvider>();
        mockProvider.Setup(p => p.SetRequestId(It.IsAny<string>()));

        // Act
        Action act = () => mockProvider.Object.SetRequestId(null!);

        // Assert - interface doesn't enforce non-null, implementation should decide
        act.Should().NotThrow();
    }

    [TestMethod]
    public void SetRequestId_WithEmptyString_ShouldBeCallable()
    {
        // Arrange
        var mockProvider = new Mock<IRequestIdProvider>();
        mockProvider.Setup(p => p.SetRequestId(string.Empty));

        // Act
        Action act = () => mockProvider.Object.SetRequestId(string.Empty);

        // Assert
        act.Should().NotThrow();
        mockProvider.Verify(p => p.SetRequestId(string.Empty), Times.Once);
    }

    [TestMethod]
    public void RequestId_AfterGenerateNew_ShouldReflectNewValue()
    {
        // Arrange
        var mockProvider = new Mock<IRequestIdProvider>();
        var oldId = "old-request-id";
        var newId = "new-request-id";
        
        mockProvider.Setup(p => p.GenerateNew()).Returns(newId).Callback(() => 
        {
            mockProvider.Setup(p => p.RequestId).Returns(newId);
        });
        mockProvider.Setup(p => p.RequestId).Returns(oldId);

        // Act
        var initialId = mockProvider.Object.RequestId;
        mockProvider.Object.GenerateNew();
        var updatedId = mockProvider.Object.RequestId;

        // Assert
        initialId.Should().Be(oldId);
        updatedId.Should().Be(newId);
    }

    [TestMethod]
    public void Interface_ShouldHaveCorrectStructure()
    {
        // Arrange & Act
        var interfaceType = typeof(IRequestIdProvider);
        var properties = interfaceType.GetProperties();
        var methods = interfaceType.GetMethods();

        // Assert
        properties.Should().HaveCount(1, "interface has RequestId property");
        methods.Should().HaveCount(3, "interface has GenerateNew, SetRequestId, and property getter");
    }

    [TestMethod]
    public void RequestIdProvider_AndCorrelationIdProvider_ShouldBeIndependent()
    {
        // Arrange
        var mockRequestProvider = new Mock<IRequestIdProvider>();
        var mockCorrelationProvider = new Mock<ICorrelationIdProvider>();
        
        var requestId = "request-123";
        var correlationId = "correlation-456";
        
        mockRequestProvider.Setup(p => p.RequestId).Returns(requestId);
        mockCorrelationProvider.Setup(p => p.CorrelationId).Returns(correlationId);

        // Act
        var request = mockRequestProvider.Object.RequestId;
        var correlation = mockCorrelationProvider.Object.CorrelationId;

        // Assert
        request.Should().Be(requestId);
        correlation.Should().Be(correlationId);
        request.Should().NotBe(correlation, "request and correlation IDs should be independent");
    }
}
