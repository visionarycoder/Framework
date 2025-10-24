using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisionaryCoder.Framework.Services.FileSystem;

namespace VisionaryCoder.Framework.Tests.FileSystem;

/// <summary>
/// Data-driven unit tests for the <see cref="FileSystemImplementation"/> record.
/// Tests file system implementation registration with various scenarios.
/// </summary>
[TestClass]
public class FileSystemImplementationTests
{
    #region Constructor Tests

    [TestMethod]
    public void Constructor_WithImplementationType_ShouldSetProperties()
    {
        // Arrange
        var implementationType = typeof(TestFileSystemProvider);

        // Act
        var implementation = new FileSystemImplementation(implementationType);

        // Assert
        implementation.ImplementationType.Should().Be(implementationType);
        implementation.Options.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithImplementationTypeAndOptions_ShouldSetBothProperties()
    {
        // Arrange
        var implementationType = typeof(TestFileSystemProvider);
        var options = new TestOptions { Setting = "value" };

        // Act
        var implementation = new FileSystemImplementation(implementationType, options);

        // Assert
        implementation.ImplementationType.Should().Be(implementationType);
        implementation.Options.Should().BeSameAs(options);
    }

    [TestMethod]
    public void Constructor_WithNullOptions_ShouldAcceptNull()
    {
        // Arrange
        var implementationType = typeof(TestFileSystemProvider);

        // Act
        var implementation = new FileSystemImplementation(implementationType, null);

        // Assert
        implementation.ImplementationType.Should().Be(implementationType);
        implementation.Options.Should().BeNull();
    }

    #endregion

    #region ImplementationType Property Tests

    [TestMethod]
    public void ImplementationType_ShouldReturnCorrectType()
    {
        // Arrange
        var implementationType = typeof(TestFileSystemProvider);
        var implementation = new FileSystemImplementation(implementationType);

        // Assert
        implementation.ImplementationType.Should().Be(implementationType);
        implementation.ImplementationType.Name.Should().Be("TestFileSystemProvider");
    }

    [TestMethod]
    public void ImplementationType_WithDifferentTypes_ShouldWork()
    {
        // Arrange
        var type1 = typeof(TestFileSystemProvider);
        var type2 = typeof(AnotherTestProvider);

        // Act
        var implementation1 = new FileSystemImplementation(type1);
        var implementation2 = new FileSystemImplementation(type2);

        // Assert
        implementation1.ImplementationType.Should().NotBe(implementation2.ImplementationType);
    }

    #endregion

    #region Options Property Tests

    [TestMethod]
    public void Options_WhenNull_ShouldBeNull()
    {
        // Arrange
        var implementation = new FileSystemImplementation(typeof(TestFileSystemProvider));

        // Assert
        implementation.Options.Should().BeNull();
    }

    [TestMethod]
    public void Options_WithValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var options = new TestOptions { Setting = "test" };
        var implementation = new FileSystemImplementation(typeof(TestFileSystemProvider), options);

        // Assert
        implementation.Options.Should().BeSameAs(options);
    }

    [TestMethod]
    public void Options_WithDifferentTypes_ShouldWork()
    {
        // Arrange
        var stringOption = "string-option";
        var intOption = 42;
        var objectOption = new { Key = "Value" };

        // Act
        var impl1 = new FileSystemImplementation(typeof(TestFileSystemProvider), stringOption);
        var impl2 = new FileSystemImplementation(typeof(TestFileSystemProvider), intOption);
        var impl3 = new FileSystemImplementation(typeof(TestFileSystemProvider), objectOption);

        // Assert
        impl1.Options.Should().Be(stringOption);
        impl2.Options.Should().Be(intOption);
        impl3.Options.Should().Be(objectOption);
    }

    #endregion

    #region Record Equality Tests

    [TestMethod]
    public void Equals_WithSameTypeAndNullOptions_ShouldBeEqual()
    {
        // Arrange
        var type = typeof(TestFileSystemProvider);
        var implementation1 = new FileSystemImplementation(type);
        var implementation2 = new FileSystemImplementation(type);

        // Assert
        implementation1.Should().Be(implementation2);
    }

    [TestMethod]
    public void Equals_WithSameTypeAndSameOptions_ShouldBeEqual()
    {
        // Arrange
        var type = typeof(TestFileSystemProvider);
        var options = new TestOptions { Setting = "value" };
        var implementation1 = new FileSystemImplementation(type, options);
        var implementation2 = new FileSystemImplementation(type, options);

        // Assert
        implementation1.Should().Be(implementation2);
    }

    [TestMethod]
    public void Equals_WithDifferentTypes_ShouldNotBeEqual()
    {
        // Arrange
        var implementation1 = new FileSystemImplementation(typeof(TestFileSystemProvider));
        var implementation2 = new FileSystemImplementation(typeof(AnotherTestProvider));

        // Assert
        implementation1.Should().NotBe(implementation2);
    }

    [TestMethod]
    public void Equals_WithDifferentOptions_ShouldNotBeEqual()
    {
        // Arrange
        var type = typeof(TestFileSystemProvider);
        var options1 = new TestOptions { Setting = "value1" };
        var options2 = new TestOptions { Setting = "value2" };
        var implementation1 = new FileSystemImplementation(type, options1);
        var implementation2 = new FileSystemImplementation(type, options2);

        // Assert
        implementation1.Should().NotBe(implementation2);
    }

    #endregion

    #region GetHashCode Tests

    [TestMethod]
    public void GetHashCode_WithSameValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var type = typeof(TestFileSystemProvider);
        var options = new TestOptions { Setting = "value" };
        var implementation1 = new FileSystemImplementation(type, options);
        var implementation2 = new FileSystemImplementation(type, options);

        // Assert
        implementation1.GetHashCode().Should().Be(implementation2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_WithDifferentTypes_ShouldReturnDifferentHashCodes()
    {
        // Arrange
        var implementation1 = new FileSystemImplementation(typeof(TestFileSystemProvider));
        var implementation2 = new FileSystemImplementation(typeof(AnotherTestProvider));

        // Assert
        implementation1.GetHashCode().Should().NotBe(implementation2.GetHashCode());
    }

    #endregion

    #region ToString Tests

    [TestMethod]
    public void ToString_ShouldIncludeImplementationType()
    {
        // Arrange
        var implementation = new FileSystemImplementation(typeof(TestFileSystemProvider));

        // Act
        var result = implementation.ToString();

        // Assert
        result.Should().Contain("TestFileSystemProvider");
    }

    [TestMethod]
    public void ToString_WithOptions_ShouldIncludeOptions()
    {
        // Arrange
        var options = new TestOptions { Setting = "test" };
        var implementation = new FileSystemImplementation(typeof(TestFileSystemProvider), options);

        // Act
        var result = implementation.ToString();

        // Assert
        result.Should().Contain("TestFileSystemProvider");
        result.Should().Contain("Options");
    }

    #endregion

    #region Deconstruction Tests

    [TestMethod]
    public void Deconstruct_ShouldExtractBothProperties()
    {
        // Arrange
        var type = typeof(TestFileSystemProvider);
        var options = new TestOptions { Setting = "value" };
        var implementation = new FileSystemImplementation(type, options);

        // Act
        var (implementationType, extractedOptions) = implementation;

        // Assert
        implementationType.Should().Be(type);
        extractedOptions.Should().BeSameAs(options);
    }

    [TestMethod]
    public void Deconstruct_WithNullOptions_ShouldWork()
    {
        // Arrange
        var type = typeof(TestFileSystemProvider);
        var implementation = new FileSystemImplementation(type);

        // Act
        var (implementationType, options) = implementation;

        // Assert
        implementationType.Should().Be(type);
        options.Should().BeNull();
    }

    #endregion

    #region With Expression Tests

    [TestMethod]
    public void WithExpression_ModifyingImplementationType_ShouldCreateNewInstance()
    {
        // Arrange
        var original = new FileSystemImplementation(typeof(TestFileSystemProvider), "options");
        var newType = typeof(AnotherTestProvider);

        // Act
        var modified = original with { ImplementationType = newType };

        // Assert
        modified.ImplementationType.Should().Be(newType);
        modified.Options.Should().Be("options");
        original.ImplementationType.Should().Be(typeof(TestFileSystemProvider));
    }

    [TestMethod]
    public void WithExpression_ModifyingOptions_ShouldCreateNewInstance()
    {
        // Arrange
        var original = new FileSystemImplementation(typeof(TestFileSystemProvider), "original");
        var newOptions = "modified";

        // Act
        var modified = original with { Options = newOptions };

        // Assert
        modified.Options.Should().Be(newOptions);
        modified.ImplementationType.Should().Be(typeof(TestFileSystemProvider));
        original.Options.Should().Be("original");
    }

    #endregion

    #region Edge Cases Tests

    [TestMethod]
    public void Constructor_WithAbstractType_ShouldAccept()
    {
        // Arrange
        var abstractType = typeof(AbstractTestProvider);

        // Act
        var implementation = new FileSystemImplementation(abstractType);

        // Assert
        implementation.ImplementationType.Should().Be(abstractType);
    }

    [TestMethod]
    public void Constructor_WithInterfaceType_ShouldAccept()
    {
        // Arrange
        var interfaceType = typeof(ITestProvider);

        // Act
        var implementation = new FileSystemImplementation(interfaceType);

        // Assert
        implementation.ImplementationType.Should().Be(interfaceType);
    }

    [TestMethod]
    public void Constructor_WithGenericType_ShouldWork()
    {
        // Arrange
        var genericType = typeof(GenericTestProvider<string>);

        // Act
        var implementation = new FileSystemImplementation(genericType);

        // Assert
        implementation.ImplementationType.Should().Be(genericType);
    }

    #endregion

    #region Type System Tests

    [TestMethod]
    public void FileSystemImplementation_ShouldBeRecord()
    {
        // Arrange & Act
        var type = typeof(FileSystemImplementation);

        // Assert
        type.GetMethod("<Clone>$", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Should().NotBeNull("records have a public <Clone>$ method");
    }

    [TestMethod]
    public void FileSystemImplementation_ShouldBeSealed()
    {
        // Arrange & Act
        var type = typeof(FileSystemImplementation);

        // Assert
        type.IsSealed.Should().BeTrue();
    }

    #endregion

    #region Test Helper Classes

    private class TestFileSystemProvider { }
    private class AnotherTestProvider { }
    private abstract class AbstractTestProvider { }
    private interface ITestProvider { }
    private class GenericTestProvider<T> { }
    private class TestOptions
    {
        public string Setting { get; set; } = string.Empty;
    }

    #endregion
}
