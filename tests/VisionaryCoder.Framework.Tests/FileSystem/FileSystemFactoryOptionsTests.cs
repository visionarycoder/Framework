using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisionaryCoder.Framework.Services.FileSystem;

namespace VisionaryCoder.Framework.Tests.FileSystem;

/// <summary>
/// Data-driven unit tests for <see cref="FileSystemFactoryOptions"/> class.
/// Tests file system factory configuration with various scenarios.
/// </summary>
[TestClass]
public class FileSystemFactoryOptionsTests
{
    #region Constructor Tests

    [TestMethod]
    public void Constructor_ShouldInitializeEmptyImplementations()
    {
        // Act
        var options = new FileSystemFactoryOptions();

        // Assert
        options.Implementations.Should().NotBeNull();
        options.Implementations.Should().BeEmpty();
    }

    #endregion

    #region Implementations Property Tests

    [TestMethod]
    public void Implementations_ShouldBeReadOnly()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();

        // Assert
        options.Implementations.Should().BeAssignableTo<IReadOnlyDictionary<string, FileSystemImplementation>>();
    }

    [TestMethod]
    public void Implementations_AfterRegistration_ShouldContainImplementation()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var implementationType = typeof(TestFileSystemProvider);
        
        // Use reflection to call internal method for testing
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method?.Invoke(options, new object?[] { "test", implementationType, null });

        // Assert
        options.Implementations.Should().ContainKey("test");
        options.Implementations["test"].ImplementationType.Should().Be(implementationType);
    }

    #endregion

    #region RegisterImplementation Tests

    [TestMethod]
    public void RegisterImplementation_WithValidParameters_ShouldAddImplementation()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var implementationType = typeof(TestFileSystemProvider);
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method?.Invoke(options, new object?[] { "local", implementationType, null });

        // Assert
        options.Implementations.Should().HaveCount(1);
        options.Implementations.Should().ContainKey("local");
        options.Implementations["local"].ImplementationType.Should().Be(implementationType);
        options.Implementations["local"].Options.Should().BeNull();
    }

    [TestMethod]
    public void RegisterImplementation_WithOptions_ShouldStoreOptions()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var implementationType = typeof(TestFileSystemProvider);
        var testOptions = new TestOptions { Setting = "value" };
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method?.Invoke(options, new object?[] { "ftp", implementationType, testOptions });

        // Assert
        options.Implementations["ftp"].Options.Should().BeSameAs(testOptions);
    }

    [TestMethod]
    public void RegisterImplementation_WithMultipleImplementations_ShouldStoreAll()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var type1 = typeof(TestFileSystemProvider);
        var type2 = typeof(AnotherTestProvider);
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method?.Invoke(options, new object?[] { "local", type1, null });
        method?.Invoke(options, new object?[] { "ftp", type2, null });

        // Assert
        options.Implementations.Should().HaveCount(2);
        options.Implementations["local"].ImplementationType.Should().Be(type1);
        options.Implementations["ftp"].ImplementationType.Should().Be(type2);
    }

    [TestMethod]
    public void RegisterImplementation_WithDuplicateName_ShouldOverwrite()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var type1 = typeof(TestFileSystemProvider);
        var type2 = typeof(AnotherTestProvider);
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method?.Invoke(options, new object?[] { "provider", type1, null });
        method?.Invoke(options, new object?[] { "provider", type2, null });

        // Assert
        options.Implementations.Should().HaveCount(1);
        options.Implementations["provider"].ImplementationType.Should().Be(type2);
    }

    [TestMethod]
    public void RegisterImplementation_WithDifferentOptionTypes_ShouldWork()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var implementationType = typeof(TestFileSystemProvider);
        var stringOptions = "string-option";
        var intOptions = 42;
        var objectOptions = new { Key = "Value" };
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method?.Invoke(options, new object?[] { "string", implementationType, stringOptions });
        method?.Invoke(options, new object?[] { "int", implementationType, intOptions });
        method?.Invoke(options, new object?[] { "object", implementationType, objectOptions });

        // Assert
        options.Implementations["string"].Options.Should().Be(stringOptions);
        options.Implementations["int"].Options.Should().Be(intOptions);
        options.Implementations["object"].Options.Should().Be(objectOptions);
    }

    #endregion

    #region Dictionary Behavior Tests

    [TestMethod]
    public void Implementations_ShouldSupportKeyEnumeration()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(options, new object?[] { "local", typeof(TestFileSystemProvider), null });
        method?.Invoke(options, new object?[] { "ftp", typeof(AnotherTestProvider), null });

        // Act
        var keys = options.Implementations.Keys.ToList();

        // Assert
        keys.Should().HaveCount(2);
        keys.Should().Contain("local");
        keys.Should().Contain("ftp");
    }

    [TestMethod]
    public void Implementations_ShouldSupportValueEnumeration()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var type1 = typeof(TestFileSystemProvider);
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(options, new object?[] { "local", type1, null });

        // Act
        var values = options.Implementations.Values.ToList();

        // Assert
        values.Should().HaveCount(1);
        values[0].ImplementationType.Should().Be(type1);
    }

    [TestMethod]
    public void Implementations_TryGetValue_ShouldWorkCorrectly()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var implementationType = typeof(TestFileSystemProvider);
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(options, new object?[] { "test", implementationType, null });

        // Act
        var exists = options.Implementations.TryGetValue("test", out var implementation);
        var notExists = options.Implementations.TryGetValue("missing", out var missing);

        // Assert
        exists.Should().BeTrue();
        implementation.Should().NotBeNull();
        implementation!.ImplementationType.Should().Be(implementationType);
        notExists.Should().BeFalse();
        missing.Should().BeNull();
    }

    [TestMethod]
    public void Implementations_ContainsKey_ShouldWorkCorrectly()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(options, new object?[] { "exists", typeof(TestFileSystemProvider), null });

        // Act & Assert
        options.Implementations.ContainsKey("exists").Should().BeTrue();
        options.Implementations.ContainsKey("missing").Should().BeFalse();
    }

    #endregion

    #region Edge Cases Tests

    [TestMethod]
    public void RegisterImplementation_WithEmptyName_ShouldStore()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method?.Invoke(options, new object?[] { "", typeof(TestFileSystemProvider), null });

        // Assert
        options.Implementations.Should().ContainKey("");
    }

    [TestMethod]
    public void RegisterImplementation_WithWhitespaceName_ShouldStore()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method?.Invoke(options, new object?[] { "  ", typeof(TestFileSystemProvider), null });

        // Assert
        options.Implementations.Should().ContainKey("  ");
    }

    [TestMethod]
    public void RegisterImplementation_WithCaseSensitiveNames_ShouldStoreSeparately()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method?.Invoke(options, new object?[] { "Provider", typeof(TestFileSystemProvider), null });
        method?.Invoke(options, new object?[] { "provider", typeof(AnotherTestProvider), null });

        // Assert
        options.Implementations.Should().HaveCount(2);
        options.Implementations["Provider"].ImplementationType.Should().Be(typeof(TestFileSystemProvider));
        options.Implementations["provider"].ImplementationType.Should().Be(typeof(AnotherTestProvider));
    }

    [TestMethod]
    public void RegisterImplementation_WithNullOptions_ShouldAccept()
    {
        // Arrange
        var options = new FileSystemFactoryOptions();
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method?.Invoke(options, new object?[] { "test", typeof(TestFileSystemProvider), null });

        // Assert
        options.Implementations["test"].Options.Should().BeNull();
    }

    [TestMethod]
    public void MultipleInstances_ShouldBeIndependent()
    {
        // Arrange
        var options1 = new FileSystemFactoryOptions();
        var options2 = new FileSystemFactoryOptions();
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        method?.Invoke(options1, new object?[] { "test", typeof(TestFileSystemProvider), null });
        method?.Invoke(options2, new object?[] { "other", typeof(AnotherTestProvider), null });

        // Assert
        options1.Implementations.Should().HaveCount(1);
        options1.Implementations.Should().ContainKey("test");
        options2.Implementations.Should().HaveCount(1);
        options2.Implementations.Should().ContainKey("other");
    }

    #endregion

    #region Type System Tests

    [TestMethod]
    public void FileSystemFactoryOptions_ShouldBeSealed()
    {
        // Arrange & Act
        var type = typeof(FileSystemFactoryOptions);

        // Assert
        type.IsSealed.Should().BeTrue();
    }

    [TestMethod]
    public void RegisterImplementation_ShouldBeInternal()
    {
        // Arrange & Act
        var method = typeof(FileSystemFactoryOptions).GetMethod("RegisterImplementation",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Assert
        method.Should().NotBeNull();
        method!.IsAssembly.Should().BeTrue(); // Internal methods are marked as Assembly
    }

    #endregion

    #region Test Helper Classes

    private class TestFileSystemProvider { }
    private class AnotherTestProvider { }
    private class TestOptions
    {
        public string Setting { get; set; } = string.Empty;
    }

    #endregion
}
