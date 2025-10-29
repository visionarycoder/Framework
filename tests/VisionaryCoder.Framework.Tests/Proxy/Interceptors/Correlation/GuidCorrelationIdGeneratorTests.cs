using FluentAssertions;
using VisionaryCoder.Framework.Proxy.Interceptors.Correlation;

namespace VisionaryCoder.Framework.Tests.Proxy.Interceptors.Correlation;

[TestClass]
public class GuidCorrelationIdGeneratorTests
{
    private GuidCorrelationIdGenerator generator = null!;

    [TestInitialize]
    public void Setup()
    {
        generator = new GuidCorrelationIdGenerator();
    }

    [TestMethod]
    public void GenerateCorrelationId_ShouldReturnNonEmptyString()
    {
        // Act
        var correlationId = generator.GenerateCorrelationId();

        // Assert
        correlationId.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public void GenerateCorrelationId_ShouldReturnValidGuid()
    {
        // Act
        var correlationId = generator.GenerateCorrelationId();

        // Assert
        Guid.TryParse(correlationId, out Guid parsedGuid).Should().BeTrue();
        parsedGuid.Should().NotBeEmpty();
    }

    [TestMethod]
    public void GenerateCorrelationId_ShouldUseHyphenatedFormat()
    {
        // Act
        var correlationId = generator.GenerateCorrelationId();

        // Assert - format "D" produces lowercase with hyphens (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)
        correlationId.Should().MatchRegex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$");
    }

    [TestMethod]
    public void GenerateCorrelationId_CalledMultipleTimes_ShouldReturnUniqueIds()
    {
        // Arrange & Act
        var ids = Enumerable.Range(0, 100)
            .Select(_ => generator.GenerateCorrelationId())
            .ToList();

        // Assert
        ids.Should().OnlyHaveUniqueItems();
        ids.Should().HaveCount(100);
    }

    [TestMethod]
    public void GenerateCorrelationId_CalledConcurrently_ShouldReturnUniqueIds()
    {
        // Arrange
        var ids = new System.Collections.Concurrent.ConcurrentBag<string>();

        // Act - call concurrently
        Parallel.For(0, 1000, _ =>
        {
            ids.Add(generator.GenerateCorrelationId());
        });

        // Assert
        ids.Should().OnlyHaveUniqueItems();
        ids.Should().HaveCount(1000);
    }

    [TestMethod]
    public void GenerateCorrelationId_ShouldAlwaysReturn36Characters()
    {
        // Arrange & Act
        var ids = Enumerable.Range(0, 10)
            .Select(_ => generator.GenerateCorrelationId())
            .ToList();

        // Assert - GUID format D is always 36 characters (32 hex + 4 hyphens)
        ids.Should().AllSatisfy(id => id.Length.Should().Be(36));
    }

    [TestMethod]
    public void GenerateCorrelationId_ShouldUseLowercase()
    {
        // Arrange & Act
        var ids = Enumerable.Range(0, 10)
            .Select(_ => generator.GenerateCorrelationId())
            .ToList();

        // Assert - format "D" produces lowercase
        ids.Should().AllSatisfy(id => id.Should().BeEquivalentTo(id.ToLowerInvariant()));
    }

    [TestMethod]
    public void GenerateCorrelationId_ShouldNotContainBraces()
    {
        // Arrange & Act
        var ids = Enumerable.Range(0, 10)
            .Select(_ => generator.GenerateCorrelationId())
            .ToList();

        // Assert - format "D" does not include braces
        ids.Should().AllSatisfy(id =>
        {
            id.Should().NotContain("{");
            id.Should().NotContain("}");
        });
    }

    [TestMethod]
    public void GenerateCorrelationId_MultipleGenerators_ShouldProduceUniqueIds()
    {
        // Arrange
        var generator1 = new GuidCorrelationIdGenerator();
        var generator2 = new GuidCorrelationIdGenerator();
        var generator3 = new GuidCorrelationIdGenerator();

        // Act
        var ids = new List<string>
        {
            generator1.GenerateCorrelationId(),
            generator2.GenerateCorrelationId(),
            generator3.GenerateCorrelationId(),
            generator1.GenerateCorrelationId(),
            generator2.GenerateCorrelationId(),
            generator3.GenerateCorrelationId()
        };

        // Assert
        ids.Should().OnlyHaveUniqueItems();
    }

    [TestMethod]
    public void GenerateCorrelationId_ShouldBeThreadSafe()
    {
        // Arrange
        var errors = new System.Collections.Concurrent.ConcurrentBag<Exception>();
        var ids = new System.Collections.Concurrent.ConcurrentBag<string>();

        // Act - stress test with many concurrent calls
        Parallel.For(0, 10000, _ =>
        {
            try
            {
                var id = generator.GenerateCorrelationId();
                ids.Add(id);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        });

        // Assert
        errors.Should().BeEmpty();
        ids.Should().HaveCount(10000);
        ids.Should().OnlyHaveUniqueItems();
    }
}
