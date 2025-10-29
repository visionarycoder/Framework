using FluentAssertions;
using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Interceptors.Telemetry.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Tests.Interceptors.Telemetry;

[TestClass]
public class NullTelemetryInterceptorTests
{
    [TestMethod]
    public void Order_ShouldBeNegative50()
    {
        // Arrange
        var interceptor = new NullTelemetryInterceptor();

        // Act & Assert
        interceptor.Order.Should().Be(-50);
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldPassThroughToNext()
    {
        // Arrange
        var interceptor = new NullTelemetryInterceptor();
        var context = new ProxyContext { MethodName = "TrackedMethod" };
        var expectedData = new List<int> { 1, 2, 3 };
        bool wasCalled = false;

        Task<Response<List<int>>> next(ProxyContext ctx, CancellationToken ct)
        {
            wasCalled = true;
            return Task.FromResult(Response<List<int>>.Success(expectedData));
        }

        // Act
        var result = await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        wasCalled.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeSameAs(expectedData);
    }

    [TestMethod]
    public async Task InvokeAsync_WithCancellationToken_ShouldPassThrough()
    {
        // Arrange
        var interceptor = new NullTelemetryInterceptor();
        var context = new ProxyContext();
        var cts = new CancellationTokenSource();
        CancellationToken receivedToken = default;

        Task<Response<decimal>> next(ProxyContext ctx, CancellationToken ct)
        {
            receivedToken = ct;
            return Task.FromResult(Response<decimal>.Success(3.14m));
        }

        // Act
        await interceptor.InvokeAsync(context, next, cts.Token);

        // Assert
        receivedToken.Should().Be(cts.Token);
    }
}
