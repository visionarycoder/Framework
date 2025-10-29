using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Interceptors.Correlation;
using ICorrelationContext = VisionaryCoder.Framework.Proxy.Abstractions.ICorrelationContext;
using ICorrelationIdGenerator = VisionaryCoder.Framework.Proxy.Interceptors.Correlation.ICorrelationIdGenerator;

namespace VisionaryCoder.Framework.Proxy.Tests.Interceptors.Correlation;

[TestClass]
public class CorrelationInterceptorTests
{
    private Mock<ILogger<CorrelationInterceptor>> mockLogger = null!;
    private Mock<ICorrelationContext> mockCorrelationContext = null!;
    private Mock<ICorrelationIdGenerator> mockIdGenerator = null!;
    private CorrelationInterceptor interceptor = null!;

    [TestInitialize]
    public void Setup()
    {
        mockLogger = new Mock<ILogger<CorrelationInterceptor>>();
        mockCorrelationContext = new Mock<ICorrelationContext>();
        mockIdGenerator = new Mock<ICorrelationIdGenerator>();
        
        interceptor = new CorrelationInterceptor(
            mockLogger.Object,
            mockCorrelationContext.Object,
            mockIdGenerator.Object);
    }

    [TestMethod]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new CorrelationInterceptor(null!, mockCorrelationContext.Object, mockIdGenerator.Object));
    }

    [TestMethod]
    public void Constructor_WithNullCorrelationContext_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new CorrelationInterceptor(mockLogger.Object, null!, mockIdGenerator.Object));
    }

    [TestMethod]
    public void Constructor_WithNullIdGenerator_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new CorrelationInterceptor(mockLogger.Object, mockCorrelationContext.Object, null!));
    }

    [TestMethod]
    public void Order_ShouldBeZero()
    {
        // Assert
        interceptor.Order.Should().Be(0);
    }

    [TestMethod]
    public async Task InvokeAsync_WithExistingCorrelationId_ShouldUseExisting()
    {
        // Arrange
        string existingCorrelationId = "existing-123";
        mockCorrelationContext.Setup(c => c.CorrelationId).Returns(existingCorrelationId);
        
        var context = new ProxyContext { MethodName = "TestMethod" };
        bool wasCalled = false;

        Task<Response<string>> next(ProxyContext ctx, CancellationToken ct)
        {
            wasCalled = true;
            return Task.FromResult(Response<string>.Success("data"));
        }

        // Act
        var result = await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        wasCalled.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        context.Items["CorrelationId"].Should().Be(existingCorrelationId);
        mockIdGenerator.Verify(g => g.GenerateId(), Times.Never);
        mockCorrelationContext.Verify(c => c.SetCorrelationId(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task InvokeAsync_WithoutCorrelationId_ShouldGenerateNew()
    {
        // Arrange
        string generatedId = "generated-456";
        mockCorrelationContext.Setup(c => c.CorrelationId).Returns((string?)null);
        mockIdGenerator.Setup(g => g.GenerateId()).Returns(generatedId);
        
        var context = new ProxyContext();

        Task<Response<int>> next(ProxyContext ctx, CancellationToken ct) =>
            Task.FromResult(Response<int>.Success(42));

        // Act
        var result = await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        context.Items["CorrelationId"].Should().Be(generatedId);
        mockIdGenerator.Verify(g => g.GenerateId(), Times.Once);
        mockCorrelationContext.Verify(c => c.SetCorrelationId(generatedId), Times.Once);
    }

    [TestMethod]
    public async Task InvokeAsync_WithEmptyCorrelationId_ShouldGenerateNew()
    {
        // Arrange
        string generatedId = "new-789";
        mockCorrelationContext.Setup(c => c.CorrelationId).Returns(string.Empty);
        mockIdGenerator.Setup(g => g.GenerateId()).Returns(generatedId);
        
        var context = new ProxyContext();

        Task<Response<bool>> next(ProxyContext ctx, CancellationToken ct) =>
            Task.FromResult(Response<bool>.Success(true));

        // Act
        await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        context.Items["CorrelationId"].Should().Be(generatedId);
        mockIdGenerator.Verify(g => g.GenerateId(), Times.Once);
    }

    [TestMethod]
    public async Task InvokeAsync_WhenGeneratingId_ShouldLogDebug()
    {
        // Arrange
        string generatedId = "new-corr-id";
        mockCorrelationContext.Setup(c => c.CorrelationId).Returns((string?)null);
        mockIdGenerator.Setup(g => g.GenerateId()).Returns(generatedId);
        
        var context = new ProxyContext();

        Task<Response<string>> next(ProxyContext ctx, CancellationToken ct) =>
            Task.FromResult(Response<string>.Success("data"));

        // Act
        await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Generated new correlation ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [TestMethod]
    public async Task InvokeAsync_WhenUsingExistingId_ShouldLogDebug()
    {
        // Arrange
        string existingId = "existing-corr-id";
        mockCorrelationContext.Setup(c => c.CorrelationId).Returns(existingId);
        
        var context = new ProxyContext();

        Task<Response<string>> next(ProxyContext ctx, CancellationToken ct) =>
            Task.FromResult(Response<string>.Success("data"));

        // Act
        await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Using existing correlation ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [TestMethod]
    public async Task InvokeAsync_WhenExceptionThrown_ShouldLogErrorAndRethrow()
    {
        // Arrange
        string correlationId = "error-corr-id";
        mockCorrelationContext.Setup(c => c.CorrelationId).Returns(correlationId);
        
        var context = new ProxyContext();
        var expectedException = new InvalidOperationException("Test error");

        Task<Response<string>> next(ProxyContext ctx, CancellationToken ct) =>
            throw expectedException;

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await interceptor.InvokeAsync(context, next, CancellationToken.None));

        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error in correlation interceptor")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldReturnResultFromNext()
    {
        // Arrange
        mockCorrelationContext.Setup(c => c.CorrelationId).Returns("test-id");
        
        var context = new ProxyContext();
        var expectedData = new { Value = 100 };

        Task<Response<object>> next(ProxyContext ctx, CancellationToken ct) =>
            Task.FromResult(Response<object>.Success(expectedData));

        // Act
        var result = await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeSameAs(expectedData);
    }

    [TestMethod]
    public async Task InvokeAsync_WithCancellationToken_ShouldPassThrough()
    {
        // Arrange
        mockCorrelationContext.Setup(c => c.CorrelationId).Returns("test-id");
        
        var context = new ProxyContext();
        var cts = new CancellationTokenSource();
        CancellationToken receivedToken = default;

        Task<Response<int>> next(ProxyContext ctx, CancellationToken ct)
        {
            receivedToken = ct;
            return Task.FromResult(Response<int>.Success(1));
        }

        // Act
        await interceptor.InvokeAsync(context, next, cts.Token);

        // Assert
        receivedToken.Should().Be(cts.Token);
    }
}
