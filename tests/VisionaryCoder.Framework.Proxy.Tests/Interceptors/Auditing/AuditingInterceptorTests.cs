using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Interceptors.Auditing;

namespace VisionaryCoder.Framework.Proxy.Tests.Interceptors.Auditing;

[TestClass]
public class AuditingInterceptorTests
{
    private Mock<ILogger<AuditingInterceptor>> mockLogger = null!;
    private Mock<IAuditSink> mockAuditSink = null!;
    private AuditingInterceptor interceptor = null!;

    [TestInitialize]
    public void Setup()
    {
        mockLogger = new Mock<ILogger<AuditingInterceptor>>();
        mockAuditSink = new Mock<IAuditSink>();
        interceptor = new AuditingInterceptor(mockLogger.Object, new[] { mockAuditSink.Object });
    }

    [TestMethod]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new AuditingInterceptor(null!, new[] { mockAuditSink.Object }));
    }

    [TestMethod]
    public void Constructor_WithNullAuditSinks_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new AuditingInterceptor(mockLogger.Object, null!));
    }

    [TestMethod]
    public void Order_ShouldBe300()
    {
        // Assert
        interceptor.Order.Should().Be(300);
    }

    [TestMethod]
    public async Task InvokeAsync_WithSuccessfulOperation_ShouldEmitSuccessAuditRecord()
    {
        // Arrange
        var context = new ProxyContext { Request = new { Id = 1 } };
        context.Items["CorrelationId"] = "test-corr-id";
        AuditRecord? capturedRecord = null;

        mockAuditSink.Setup(s => s.WriteAsync(It.IsAny<AuditRecord>(), It.IsAny<CancellationToken>()))
            .Callback<AuditRecord, CancellationToken>((record, ct) => capturedRecord = record)
            .Returns(Task.CompletedTask);

        Task<Response<string>> next(ProxyContext ctx, CancellationToken ct) =>
            Task.FromResult(Response<string>.Success("data"));

        // Act
        var result = await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedRecord.Should().NotBeNull();
        capturedRecord!.Success.Should().BeTrue();
        capturedRecord.CorrelationId.Should().Be("test-corr-id");
        capturedRecord.ErrorMessage.Should().BeNull();
        capturedRecord.Duration.Should().BeGreaterOrEqualTo(TimeSpan.Zero);
    }

    [TestMethod]
    public async Task InvokeAsync_WithFailedResponse_ShouldEmitFailureAuditRecord()
    {
        // Arrange
        var context = new ProxyContext { Request = new { Id = 1 } };
        context.Items["CorrelationId"] = "fail-corr-id";
        AuditRecord? capturedRecord = null;

        mockAuditSink.Setup(s => s.WriteAsync(It.IsAny<AuditRecord>(), It.IsAny<CancellationToken>()))
            .Callback<AuditRecord, CancellationToken>((record, ct) => capturedRecord = record)
            .Returns(Task.CompletedTask);

        Task<Response<int>> next(ProxyContext ctx, CancellationToken ct) =>
            Task.FromResult(Response<int>.Failure("Test error"));

        // Act
        var result = await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        capturedRecord.Should().NotBeNull();
        capturedRecord!.Success.Should().BeFalse();
        capturedRecord.ErrorMessage.Should().Be("Operation failed");
    }

    [TestMethod]
    public async Task InvokeAsync_WithException_ShouldEmitExceptionAuditRecord()
    {
        // Arrange
        var context = new ProxyContext { Request = new { Id = 1 } };
        context.Items["CorrelationId"] = "exception-corr-id";
        var expectedException = new InvalidOperationException("Test exception");
        AuditRecord? capturedRecord = null;

        mockAuditSink.Setup(s => s.WriteAsync(It.IsAny<AuditRecord>(), It.IsAny<CancellationToken>()))
            .Callback<AuditRecord, CancellationToken>((record, ct) => capturedRecord = record)
            .Returns(Task.CompletedTask);

        Task<Response<string>> next(ProxyContext ctx, CancellationToken ct) =>
            throw expectedException;

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await interceptor.InvokeAsync(context, next, CancellationToken.None));

        capturedRecord.Should().NotBeNull();
        capturedRecord!.Success.Should().BeFalse();
        capturedRecord.ErrorMessage.Should().Be("Test exception");
        capturedRecord.ExceptionType.Should().Be("InvalidOperationException");
    }

    [TestMethod]
    public async Task InvokeAsync_WithNoCorrelationId_ShouldGenerateOne()
    {
        // Arrange
        var context = new ProxyContext { Request = new { Id = 1 } };
        AuditRecord? capturedRecord = null;

        mockAuditSink.Setup(s => s.WriteAsync(It.IsAny<AuditRecord>(), It.IsAny<CancellationToken>()))
            .Callback<AuditRecord, CancellationToken>((record, ct) => capturedRecord = record)
            .Returns(Task.CompletedTask);

        Task<Response<bool>> next(ProxyContext ctx, CancellationToken ct) =>
            Task.FromResult(Response<bool>.Success(true));

        // Act
        await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        capturedRecord.Should().NotBeNull();
        capturedRecord!.CorrelationId.Should().NotBeNullOrEmpty();
        Guid.TryParse(capturedRecord.CorrelationId, out _).Should().BeTrue();
    }

    [TestMethod]
    public async Task InvokeAsync_WithMultipleAuditSinks_ShouldEmitToAll()
    {
        // Arrange
        var mockSink1 = new Mock<IAuditSink>();
        var mockSink2 = new Mock<IAuditSink>();
        var multiSinkInterceptor = new AuditingInterceptor(
            mockLogger.Object,
            new[] { mockSink1.Object, mockSink2.Object });

        var context = new ProxyContext { Request = new { Id = 1 } };

        Task<Response<string>> next(ProxyContext ctx, CancellationToken ct) =>
            Task.FromResult(Response<string>.Success("data"));

        // Act
        await multiSinkInterceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        mockSink1.Verify(s => s.WriteAsync(It.IsAny<AuditRecord>(), It.IsAny<CancellationToken>()), Times.Once);
        mockSink2.Verify(s => s.WriteAsync(It.IsAny<AuditRecord>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task InvokeAsync_WhenAuditSinkFails_ShouldLogErrorButNotThrow()
    {
        // Arrange
        var context = new ProxyContext { Request = new { Id = 1 } };
        
        mockAuditSink.Setup(s => s.WriteAsync(It.IsAny<AuditRecord>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Audit sink failed"));

        Task<Response<int>> next(ProxyContext ctx, CancellationToken ct) =>
            Task.FromResult(Response<int>.Success(42));

        // Act
        var result = await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(42);
        
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to emit audit record")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [TestMethod]
    public async Task InvokeAsync_WithNullRequest_ShouldUseUnknownRequestType()
    {
        // Arrange
        var context = new ProxyContext(); // No Request
        AuditRecord? capturedRecord = null;

        mockAuditSink.Setup(s => s.WriteAsync(It.IsAny<AuditRecord>(), It.IsAny<CancellationToken>()))
            .Callback<AuditRecord, CancellationToken>((record, ct) => capturedRecord = record)
            .Returns(Task.CompletedTask);

        Task<Response<string>> next(ProxyContext ctx, CancellationToken ct) =>
            Task.FromResult(Response<string>.Success("data"));

        // Act
        await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        capturedRecord.Should().NotBeNull();
        capturedRecord!.MethodName.Should().Contain("Unknown");
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldMeasureDuration()
    {
        // Arrange
        var context = new ProxyContext { Request = new { Id = 1 } };
        AuditRecord? capturedRecord = null;

        mockAuditSink.Setup(s => s.WriteAsync(It.IsAny<AuditRecord>(), It.IsAny<CancellationToken>()))
            .Callback<AuditRecord, CancellationToken>((record, ct) => capturedRecord = record)
            .Returns(Task.CompletedTask);

        Task<Response<string>> next(ProxyContext ctx, CancellationToken ct)
        {
            Thread.Sleep(10); // Small delay
            return Task.FromResult(Response<string>.Success("data"));
        }

        // Act
        await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        capturedRecord.Should().NotBeNull();
        capturedRecord!.Duration.Should().BeGreaterThan(TimeSpan.Zero);
        capturedRecord.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        capturedRecord.CompletedAt.Should().NotBeNull();
    }

    [TestMethod]
    public async Task InvokeAsync_WithCancellationToken_ShouldPassThrough()
    {
        // Arrange
        var context = new ProxyContext { Request = new { Id = 1 } };
        var cts = new CancellationTokenSource();
        CancellationToken receivedToken = default;

        Task<Response<string>> next(ProxyContext ctx, CancellationToken ct)
        {
            receivedToken = ct;
            return Task.FromResult(Response<string>.Success("data"));
        }

        // Act
        await interceptor.InvokeAsync(context, next, cts.Token);

        // Assert
        receivedToken.Should().Be(cts.Token);
    }
}
