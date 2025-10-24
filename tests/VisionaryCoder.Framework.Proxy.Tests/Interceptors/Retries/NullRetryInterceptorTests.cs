using FluentAssertions;
using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Interceptors.Retry.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Tests.Interceptors.Retries;

[TestClass]
public class NullRetryInterceptorTests
{
    [TestMethod]
    public void Order_ShouldBe200()
    {
        // Arrange
        var interceptor = new NullRetryInterceptor();

        // Act & Assert
        interceptor.Order.Should().Be(200);
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldPassThroughToNext()
    {
        // Arrange
        var interceptor = new NullRetryInterceptor();
        var context = new ProxyContext { MethodName = "TestMethod" };
        var expectedData = "test data";
        var wasCalled = false;

        Task<Response<string>> next(ProxyContext ctx, CancellationToken ct)
        {
            wasCalled = true;
            return Task.FromResult(Response<string>.Success(expectedData));
        }

        // Act
        var result = await interceptor.InvokeAsync(context, next, CancellationToken.None);

        // Assert
        wasCalled.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(expectedData);
    }

    [TestMethod]
    public async Task InvokeAsync_WithCancellationToken_ShouldPassThrough()
    {
        // Arrange
        var interceptor = new NullRetryInterceptor();
        var context = new ProxyContext();
        var cts = new CancellationTokenSource();
        CancellationToken receivedToken = default;

        Task<Response<int>> next(ProxyContext ctx, CancellationToken ct)
        {
            receivedToken = ct;
            return Task.FromResult(Response<int>.Success(42));
        }

        // Act
        await interceptor.InvokeAsync(context, next, cts.Token);

        // Assert
        receivedToken.Should().Be(cts.Token);
    }
}
