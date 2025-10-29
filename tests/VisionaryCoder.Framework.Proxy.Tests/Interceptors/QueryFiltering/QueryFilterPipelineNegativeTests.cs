using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisionaryCoder.Framework.Proxy;
using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Querying;
using VisionaryCoder.Framework.Querying.Serialization;

namespace VisionaryCoder.Framework.Tests
{
    [TestClass]
    public sealed class QueryFilterPipelineNegativeTests
    {
        private record User(int Id, string Name, string Email);

        [DataTestMethod]
        [DataRow(@"{ ""operator"": ""FooBar"", ""property"": ""Name"", ""value"": ""ann"" }", DisplayName = "Unsupported operator")]
        [DataRow(@"{ ""operator"": ""Contains"", ""value"": ""ann"" }", DisplayName = "Missing property field")]
        [DataRow(@"{ ""operator"": ""And"", ""children"": [] }", DisplayName = "Empty children array")]
        [DataRow(@"{ ""operator"": ""Contains"", ""property"": ""Name"", ""ignoreCase"": ""yes"" }", DisplayName = "Wrong type for ignoreCase")]
        public async Task InvalidPayloads_ShouldThrowValidationError(string invalidJson)
        {
            // Arrange
            var context = new ProxyContext
            {
                Url = "http://localhost/fake",
                Method = "POST",
                Body = invalidJson,
                Headers = new Dictionary<string, string>()
            };

            var services = new ServiceCollection()
                .AddProxyPipeline()
                .AddProxyInterceptor<QueryFilterInterceptor>()
                .AddProxyTransport<FakeTransport>() // stub transport
                .BuildServiceProvider();

            var pipeline = services.GetRequiredService<IProxyPipeline>();

            // Act + Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await pipeline.SendAsync<QueryFilter<User>>(context);
            });
        }

        // Fake transport should never be reached for invalid payloads
        private sealed class FakeTransport : IProxyTransport
        {
            public Task<Response<T>> SendCoreAsync<T>(ProxyContext context, CancellationToken cancellationToken = default)
                => throw new InvalidOperationException("Transport should not be invoked for invalid payloads");
        }
    }
}
