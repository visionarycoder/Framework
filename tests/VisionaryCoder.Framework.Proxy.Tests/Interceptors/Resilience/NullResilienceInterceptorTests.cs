using FluentAssertions;
using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Proxy.Interceptors.Resilience.Abstractions;

namespace VisionaryCoder.Framework.Proxy.Tests.Interceptors.Resilience;

[TestClass]
public class NullResilienceInterceptorTests
{
    [TestMethod]
    public void Order_ShouldBe180()
    {
        // Arrange
        var interceptor = new NullResilienceInterceptor();

        // Act & Assert
        interceptor.Order.Should().Be(180);
    }

    [TestMethod]
    public async Task InvokeAsync_ShouldPassThroughToNext()
    {
        // Arrange
        var interceptor = new NullResilienceInterceptor();
        var context = new ProxyContext { MethodName = "ResilientMethod" };
        var expectedData = new { Value = 42 };
        bool wasCalled = false;

        Task<Response<object>> next(ProxyContext ctx, CancellationToken ct)
        {
            wasCalled = true;
            return Task.FromResult(Response<object>.Success(expectedData));
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
        var interceptor = new NullResilienceInterceptor();
        var context = new ProxyContext();
        var cts = new CancellationTokenSource();
        CancellationToken receivedToken = default;

        Task<Response<bool>> next(ProxyContext ctx, CancellationToken ct)
        {
            receivedToken = ct;
            return Task.FromResult(Response<bool>.Success(true));
        }

        // Act
        await interceptor.InvokeAsync(context, next, cts.Token);

        // Assert
        receivedToken.Should().Be(cts.Token);
    }
}
