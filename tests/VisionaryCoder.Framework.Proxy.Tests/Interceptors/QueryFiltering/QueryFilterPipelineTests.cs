using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisionaryCoder.Framework.Proxy;
using VisionaryCoder.Framework.Proxy.Abstractions;
using VisionaryCoder.Framework.Querying;
using VisionaryCoder.Framework.Querying.Serialization;

namespace VisionaryCoder.Framework.Tests
{
    [TestClass]
    public sealed class QueryFilterPipelinePositiveTests
    {
        private record User(int Id, string Name, string Email);

        [DataTestMethod]
        [DataRow(@"{ ""operator"": ""Contains"", ""property"": ""Name"", ""value"": ""Ann"", ""ignoreCase"": true }", 2, DisplayName = "ContainsIgnoreCase on Name")]
        [DataRow(@"{ ""operator"": ""StartsWith"", ""property"": ""Email"", ""value"": ""jo"", ""ignoreCase"": true }", 1, DisplayName = "StartsWithIgnoreCase on Email")]
        [DataRow(@"{ ""operator"": ""EndsWith"", ""property"": ""Email"", ""value"": "".org"", ""ignoreCase"": true }", 2, DisplayName = "EndsWithIgnoreCase on Email")]
        [DataRow(@"
        {
          ""operator"": ""And"",
          ""children"": [
            { ""operator"": ""Contains"", ""property"": ""Name"", ""value"": ""Ann"", ""ignoreCase"": true },
            { ""operator"": ""EndsWith"", ""property"": ""Email"", ""value"": "".org"", ""ignoreCase"": true }
          ]
        }", 2, DisplayName = "Composite And filter")]
        public async Task ValidPayloads_ShouldRoundTripAndFilter(string validJson, int expectedCount)
        {
            // Arrange
            var context = new ProxyContext
            {
                Url = "http://localhost/fake",
                Method = "POST",
                Body = validJson,
                Headers = new Dictionary<string, string>()
            };

            var services = new ServiceCollection()
                .AddProxyPipeline()
                .AddProxyInterceptor<QueryFilterInterceptor>()
                .AddProxyTransport<FakeTransport>() // stub transport
                .BuildServiceProvider();

            var pipeline = services.GetRequiredService<IProxyPipeline>();

            // Act
            Response<QueryFilter<User>> response = await pipeline.SendAsync<QueryFilter<User>>(context);

            // Assert
            Assert.IsTrue(response.IsSuccess, "Response should be successful");

            var filter = response.Data!;
            var users = new List<User>
            {
                new(1, "Ann Smith", "ann@ngo.org"),
                new(2, "Bob", "bob@gmail.com"),
                new(3, "Joanne", "joanne@company.org")
            }.AsQueryable();

            var result = users.Apply(filter).ToList();

            Assert.AreEqual(expectedCount, result.Count, "Filtered result count mismatch");
        }

        // Fake transport echoes back the rehydrated filter
        private sealed class FakeTransport : IProxyTransport
        {
            public Task<Response<T>> SendCoreAsync<T>(ProxyContext context, CancellationToken cancellationToken = default)
            {
                FilterNode node = QueryFilterSerializer.Deserialize((string)context.Body!)!;
                var filter = node.ToQueryFilter<T>();
                return Task.FromResult(Response<T>.Success(filter, 200));
            }
        }
    }
}
