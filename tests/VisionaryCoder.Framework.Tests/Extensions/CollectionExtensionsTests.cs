using FluentAssertions;

namespace VisionaryCoder.Framework.Extensions.Tests;

/// <summary>
/// Unit tests for CollectionExtensions to ensure 100% code coverage.
/// </summary>
[TestClass]
public class CollectionExtensionsTests
{
    #region IsNullOrEmpty Tests

    [TestMethod]
    public void IsNullOrEmpty_WithNullCollection_ShouldReturnTrue()
    {
        // Arrange
        ICollection<string>? collection = null;

        // Act
        var result = collection.IsNullOrEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void IsNullOrEmpty_WithEmptyCollection_ShouldReturnTrue()
    {
        // Arrange
        var collection = new List<string>();

        // Act
        var result = collection.IsNullOrEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void IsNullOrEmpty_WithCollectionContainingOnlyNulls_ShouldReturnTrue()
    {
        // Arrange
        var collection = new List<string?> { null, null, null };

        // Act
        var result = collection.IsNullOrEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void IsNullOrEmpty_WithCollectionContainingOnlyDefaults_ShouldReturnTrue()
    {
        // Arrange
        var collection = new List<int> { 0, 0, 0 };

        // Act
        var result = collection.IsNullOrEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void IsNullOrEmpty_WithCollectionContainingValidValues_ShouldReturnFalse()
    {
        // Arrange
        var collection = new List<string> { "value1", "value2" };

        // Act
        var result = collection.IsNullOrEmpty();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void IsNullOrEmpty_WithMixedValidAndDefaultValues_ShouldReturnFalse()
    {
        // Arrange
        var collection = new List<string?> { null, "valid", null };

        // Act
        var result = collection.IsNullOrEmpty();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region HasAny Tests

    [TestMethod]
    public void HasAny_WithNullCollection_ShouldReturnFalse()
    {
        // Arrange
        ICollection<string>? collection = null;

        // Act
        var result = collection.HasAny();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void HasAny_WithEmptyCollection_ShouldReturnFalse()
    {
        // Arrange
        var collection = new List<string>();

        // Act
        var result = collection.HasAny();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void HasAny_WithNonEmptyCollection_ShouldReturnTrue()
    {
        // Arrange
        var collection = new List<string> { "item" };

        // Act
        var result = collection.HasAny();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void HasAny_WithMultipleItems_ShouldReturnTrue()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3, 4, 5 };

        // Act
        var result = collection.HasAny();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region AddRange Tests

    [TestMethod]
    public void AddRange_WithValidItems_ShouldAddAllItems()
    {
        // Arrange
        var collection = new List<string> { "existing" };
        var itemsToAdd = new[] { "item1", "item2", "item3" };

        // Act
        collection.AddRange(itemsToAdd);

        // Assert
        collection.Should().HaveCount(4);
        collection.Should().Contain("existing");
        collection.Should().Contain("item1");
        collection.Should().Contain("item2");
        collection.Should().Contain("item3");
    }

    [TestMethod]
    public void AddRange_WithEmptyEnumerable_ShouldNotAddAnyItems()
    {
        // Arrange
        var collection = new List<string> { "existing" };
        var itemsToAdd = Array.Empty<string>();

        // Act
        collection.AddRange(itemsToAdd);

        // Assert
        collection.Should().HaveCount(1);
        collection.Should().Contain("existing");
    }

    [TestMethod]
    public void AddRange_WithNullCollection_ShouldThrowArgumentNullException()
    {
        // Arrange
        ICollection<string>? collection = null;
        var itemsToAdd = new[] { "item1" };

        // Act & Assert
        var action = () => collection!.AddRange(itemsToAdd);
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void AddRange_WithDuplicateItems_ShouldAddAllDuplicates()
    {
        // Arrange
        var collection = new List<string>();
        var itemsToAdd = new[] { "item", "item", "item" };

        // Act
        collection.AddRange(itemsToAdd);

        // Assert
        collection.Should().HaveCount(3);
        collection.Should().AllBe("item");
    }

    #endregion

    #region TryGetElement Tests

    [TestMethod]
    public void TryGetElement_WithValidIndex_ShouldReturnTrueAndValue()
    {
        // Arrange
        var collection = new List<string> { "first", "second", "third" };

        // Act
        var result = collection.TryGetElement(1, out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be("second");
    }

    [TestMethod]
    public void TryGetElement_WithNegativeIndex_ShouldReturnFalseAndDefault()
    {
        // Arrange
        var collection = new List<string> { "first", "second" };

        // Act
        var result = collection.TryGetElement(-1, out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [TestMethod]
    public void TryGetElement_WithIndexOutOfRange_ShouldReturnFalseAndDefault()
    {
        // Arrange
        var collection = new List<string> { "first", "second" };

        // Act
        var result = collection.TryGetElement(5, out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [TestMethod]
    public void TryGetElement_WithNullCollection_ShouldThrowArgumentNullException()
    {
        // Arrange
        ICollection<string>? collection = null;

        // Act & Assert
        var action = () => collection!.TryGetElement(0, out _);
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void TryGetElement_WithEmptyCollection_ShouldReturnFalseAndDefault()
    {
        // Arrange
        var collection = new List<string>();

        // Act
        var result = collection.TryGetElement(0, out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [TestMethod]
    public void TryGetElement_WithFirstAndLastIndex_ShouldWork()
    {
        // Arrange
        var collection = new List<int> { 10, 20, 30 };

        // Act
        var firstResult = collection.TryGetElement(0, out var firstValue);
        var lastResult = collection.TryGetElement(2, out var lastValue);

        // Assert
        firstResult.Should().BeTrue();
        firstValue.Should().Be(10);
        lastResult.Should().BeTrue();
        lastValue.Should().Be(30);
    }

    #endregion

    #region RemoveWhere Tests

    [TestMethod]
    public void RemoveWhere_WithMatchingElements_ShouldRemoveThemAndReturnCount()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3, 4, 5, 6 };

        // Act
        var removedCount = collection.RemoveWhere(x => x % 2 == 0); // Remove even numbers

        // Assert
        removedCount.Should().Be(3);
        collection.Should().HaveCount(3);
        collection.Should().Equal(1, 3, 5);
    }

    [TestMethod]
    public void RemoveWhere_WithNoMatchingElements_ShouldReturnZero()
    {
        // Arrange
        var collection = new List<string> { "apple", "banana", "cherry" };

        // Act
        var removedCount = collection.RemoveWhere(x => x.StartsWith("z"));

        // Assert
        removedCount.Should().Be(0);
        collection.Should().HaveCount(3);
        collection.Should().Equal("apple", "banana", "cherry");
    }

    [TestMethod]
    public void RemoveWhere_WithAllElementsMatching_ShouldRemoveAll()
    {
        // Arrange
        var collection = new List<int> { 2, 4, 6, 8 };

        // Act
        var removedCount = collection.RemoveWhere(x => x % 2 == 0);

        // Assert
        removedCount.Should().Be(4);
        collection.Should().BeEmpty();
    }

    [TestMethod]
    public void RemoveWhere_WithNullCollection_ShouldThrowArgumentNullException()
    {
        // Arrange
        ICollection<string>? collection = null;

        // Act & Assert
        var action = () => collection!.RemoveWhere(x => x.Length > 0);
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void RemoveWhere_WithNullPredicate_ShouldThrowArgumentNullException()
    {
        // Arrange
        var collection = new List<string> { "item" };

        // Act & Assert
        var action = () => collection.RemoveWhere(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void RemoveWhere_WithEmptyCollection_ShouldReturnZero()
    {
        // Arrange
        var collection = new List<string>();

        // Act
        var removedCount = collection.RemoveWhere(x => true);

        // Assert
        removedCount.Should().Be(0);
        collection.Should().BeEmpty();
    }

    #endregion

    #region AddIf Tests

    [TestMethod]
    public void AddIf_WithConditionTrue_ShouldAddItemAndReturnTrue()
    {
        // Arrange
        var collection = new List<int> { 1, 2 };

        // Act
        var result = collection.AddIf(3, x => x > 0);

        // Assert
        result.Should().BeTrue();
        collection.Should().HaveCount(3);
        collection.Should().Contain(3);
    }

    [TestMethod]
    public void AddIf_WithConditionFalse_ShouldNotAddItemAndReturnFalse()
    {
        // Arrange
        var collection = new List<int> { 1, 2 };

        // Act
        var result = collection.AddIf(-1, x => x > 0);

        // Assert
        result.Should().BeFalse();
        collection.Should().HaveCount(2);
        collection.Should().NotContain(-1);
    }

    [TestMethod]
    public void AddIf_WithNullCollection_ShouldThrowArgumentNullException()
    {
        // Arrange
        ICollection<string>? collection = null;

        // Act & Assert
        var action = () => collection!.AddIf("item", x => true);
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void AddIf_WithComplexCondition_ShouldWork()
    {
        // Arrange
        var collection = new List<string> { "apple", "banana" };

        // Act
        var result1 = collection.AddIf("cherry", x => x.Length > 3);
        var result2 = collection.AddIf("kiwi", x => x.Length > 5);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
        collection.Should().HaveCount(3);
        collection.Should().Contain("cherry");
        collection.Should().NotContain("kiwi");
    }

    [TestMethod]
    public void AddIf_WithNullItem_ShouldWorkIfConditionAllows()
    {
        // Arrange
        var collection = new List<string?> { "item1" };

        // Act
        var result = collection.AddIf(null, x => x == null);

        // Assert
        result.Should().BeTrue();
        collection.Should().HaveCount(2);
        collection.Should().Contain(x => x == null);
    }

    #endregion

    #region Integration Tests

    [TestMethod]
    public void CollectionExtensions_ChainedOperations_ShouldWorkCorrectly()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3, 4, 5 };

        // Act
        collection.AddRange(new[] { 6, 7, 8 });
        var evenRemoved = collection.RemoveWhere(x => x % 2 == 0);
        var added = collection.AddIf(9, x => x % 2 != 0);

        // Assert
        evenRemoved.Should().Be(4); // Removed 2, 4, 6, 8
        added.Should().BeTrue();
        collection.Should().Equal(1, 3, 5, 7, 9);
        collection.HasAny().Should().BeTrue();
        collection.IsNullOrEmpty().Should().BeFalse();
    }

    [TestMethod]
    public void CollectionExtensions_WithDifferentCollectionTypes_ShouldWork()
    {
        // Test with HashSet
        var hashSet = new HashSet<string> { "a", "b" };
        hashSet.AddRange(new[] { "c", "d" });
        hashSet.Should().HaveCount(4);

        // Test with List
        var list = new List<int> { 1, 2, 3 };
        list.TryGetElement(1, out var value).Should().BeTrue();
        value.Should().Be(2);
    }

    #endregion
}