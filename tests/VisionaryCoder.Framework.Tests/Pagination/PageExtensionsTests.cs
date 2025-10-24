using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisionaryCoder.Framework.Pagination;

namespace VisionaryCoder.Framework.Tests.Pagination;

/// <summary>
/// Data-driven unit tests for <see cref="PageExtensions"/> static class.
/// Tests pagination extension methods for IQueryable with various scenarios.
/// </summary>
[TestClass]
public class PageExtensionsTests
{
    #region ToPageAsync Tests

    [TestMethod]
    public async Task ToPageAsync_WithFirstPage_ShouldReturnCorrectPage()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageNumber: 1, pageSize: 5);

        // Act
        var result = await context.TestEntities.ToPageAsync(request);

        // Assert
        result.Items.Should().HaveCount(5);
        result.TotalCount.Should().Be(15);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(5);
        result.Items[0].Id.Should().Be(1);
        result.Items[4].Id.Should().Be(5);
    }

    [TestMethod]
    public async Task ToPageAsync_WithMiddlePage_ShouldReturnCorrectPage()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageNumber: 2, pageSize: 5);

        // Act
        var result = await context.TestEntities.ToPageAsync(request);

        // Assert
        result.Items.Should().HaveCount(5);
        result.TotalCount.Should().Be(15);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.Items[0].Id.Should().Be(6);
        result.Items[4].Id.Should().Be(10);
    }

    [TestMethod]
    public async Task ToPageAsync_WithLastPage_ShouldReturnRemainingItems()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageNumber: 3, pageSize: 5);

        // Act
        var result = await context.TestEntities.ToPageAsync(request);

        // Assert
        result.Items.Should().HaveCount(5);
        result.TotalCount.Should().Be(15);
        result.PageNumber.Should().Be(3);
        result.PageSize.Should().Be(5);
        result.Items[0].Id.Should().Be(11);
        result.Items[4].Id.Should().Be(15);
    }

    [TestMethod]
    public async Task ToPageAsync_WithPageBeyondData_ShouldReturnEmptyPage()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageNumber: 10, pageSize: 5);

        // Act
        var result = await context.TestEntities.ToPageAsync(request);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(15);
        result.PageNumber.Should().Be(10);
        result.PageSize.Should().Be(5);
    }

    [TestMethod]
    public async Task ToPageAsync_WithLargePageSize_ShouldReturnAllItems()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageNumber: 1, pageSize: 100);

        // Act
        var result = await context.TestEntities.ToPageAsync(request);

        // Assert
        result.Items.Should().HaveCount(15);
        result.TotalCount.Should().Be(15);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(100);
    }

    [TestMethod]
    public async Task ToPageAsync_WithEmptyDataset_ShouldReturnEmptyPage()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var request = new PageRequest(pageNumber: 1, pageSize: 5);

        // Act
        var result = await context.TestEntities.ToPageAsync(request);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(5);
    }

    [TestMethod]
    public async Task ToPageAsync_WithFilteredQuery_ShouldReturnFilteredPage()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageNumber: 1, pageSize: 5);

        // Act
        var result = await context.TestEntities
            .Where(e => e.Id > 5)
            .ToPageAsync(request);

        // Assert
        result.Items.Should().HaveCount(5);
        result.TotalCount.Should().Be(10);
        result.Items[0].Id.Should().Be(6);
        result.Items[4].Id.Should().Be(10);
    }

    [TestMethod]
    public async Task ToPageAsync_WithOrderedQuery_ShouldMaintainOrder()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageNumber: 1, pageSize: 5);

        // Act
        var result = await context.TestEntities
            .OrderByDescending(e => e.Id)
            .ToPageAsync(request);

        // Assert
        result.Items.Should().HaveCount(5);
        result.Items[0].Id.Should().Be(15);
        result.Items[4].Id.Should().Be(11);
    }

    [TestMethod]
    public async Task ToPageAsync_WithPageSizeOne_ShouldReturnSingleItem()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageNumber: 5, pageSize: 1);

        // Act
        var result = await context.TestEntities.ToPageAsync(request);

        // Assert
        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(15);
        result.Items[0].Id.Should().Be(5);
    }

    [TestMethod]
    public async Task ToPageAsync_WithCancellationToken_ShouldHonorCancellation()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageNumber: 1, pageSize: 5);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await FluentActions.Invoking(async () =>
            await context.TestEntities.ToPageAsync(request, cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region ToPageWithTokenAsync Tests

    [TestMethod]
    public async Task ToPageWithTokenAsync_WithCustomPagination_ShouldReturnPage()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageSize: 5);

        // Act
        var result = await context.TestEntities.ToPageWithTokenAsync(
            request,
            async (query, token, pageSize, ct) =>
            {
                var items = await query.Take(pageSize).ToListAsync(ct);
                var nextToken = items.Count == pageSize ? "next-page-token" : null;
                return (items, nextToken);
            });

        // Assert
        result.Items.Should().HaveCount(5);
        result.NextToken.Should().Be("next-page-token");
        result.PageSize.Should().Be(5);
        result.PageNumber.Should().Be(0); // Token-based doesn't use page numbers
        result.TotalCount.Should().Be(0); // Token-based doesn't calculate total
    }

    [TestMethod]
    public async Task ToPageWithTokenAsync_WithLastPage_ShouldReturnNullNextToken()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageSize: 100);

        // Act
        var result = await context.TestEntities.ToPageWithTokenAsync(
            request,
            async (query, token, pageSize, ct) =>
            {
                var items = await query.Take(pageSize).ToListAsync(ct);
                var nextToken = items.Count == pageSize ? "next-token" : null;
                return (items, nextToken);
            });

        // Assert
        result.Items.Should().HaveCount(15);
        result.NextToken.Should().BeNull();
    }

    [TestMethod]
    public async Task ToPageWithTokenAsync_WithContinuationToken_ShouldUsePreviousToken()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageSize: 5, continuationToken: "page-2");
        string? receivedToken = null;

        // Act
        var result = await context.TestEntities.ToPageWithTokenAsync(
            request,
            async (query, token, pageSize, ct) =>
            {
                receivedToken = token;
                var items = await query.Skip(5).Take(pageSize).ToListAsync(ct);
                return (items, "page-3");
            });

        // Assert
        receivedToken.Should().Be("page-2");
        result.Items.Should().HaveCount(5);
        result.NextToken.Should().Be("page-3");
    }

    [TestMethod]
    public async Task ToPageWithTokenAsync_WithEmptyResult_ShouldReturnEmptyPage()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var request = new PageRequest(pageSize: 5);

        // Act
        var result = await context.TestEntities.ToPageWithTokenAsync(
            request,
            async (query, token, pageSize, ct) =>
            {
                var items = await query.Take(pageSize).ToListAsync(ct);
                return (items, (string?)null);
            });

        // Assert
        result.Items.Should().BeEmpty();
        result.NextToken.Should().BeNull();
    }

    [TestMethod]
    public async Task ToPageWithTokenAsync_WithCancellation_ShouldHonorCancellation()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageSize: 5);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await FluentActions.Invoking(async () =>
            await context.TestEntities.ToPageWithTokenAsync(
                request,
                async (query, token, pageSize, ct) =>
                {
                    var items = await query.Take(pageSize).ToListAsync(ct);
                    return (items, "next");
                },
                cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region Edge Cases Tests

    [TestMethod]
    public async Task ToPageAsync_WithComplexQuery_ShouldWorkCorrectly()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageNumber: 1, pageSize: 3);

        // Act
        var result = await context.TestEntities
            .Where(e => e.Id % 2 == 0)
            .OrderBy(e => e.Name)
            .ToPageAsync(request);

        // Assert
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(7); // 7 even numbers in 1-15
    }

    [TestMethod]
    public async Task ToPageAsync_WithMultipleCalls_ShouldBeConsistent()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        await SeedTestData(context);
        var request = new PageRequest(pageNumber: 2, pageSize: 5);

        // Act
        var result1 = await context.TestEntities.ToPageAsync(request);
        var result2 = await context.TestEntities.ToPageAsync(request);

        // Assert
        result1.Items.Should().BeEquivalentTo(result2.Items);
        result1.TotalCount.Should().Be(result2.TotalCount);
    }

    #endregion

    #region Helper Methods & Test Context

    private static TestDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
        return new TestDbContext(options);
    }

    private static async Task SeedTestData(TestDbContext context)
    {
        for (int i = 1; i <= 15; i++)
        {
            context.TestEntities.Add(new TestEntity { Id = i, Name = $"Entity{i:D2}" });
        }
        await context.SaveChangesAsync();
    }

    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<TestEntity> TestEntities => Set<TestEntity>();
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    #endregion
}
