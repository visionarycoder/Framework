using FluentAssertions;

namespace VisionaryCoder.Framework.Extensions.Tests;

[TestClass]
public class TypeExtensionTests
{
    #region AsBoolean Tests

    [TestMethod]
    public void AsBoolean_WithNull_ShouldReturnFalse()
    {
        // Arrange
        object? value = null;

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void AsBoolean_WithBooleanTrue_ShouldReturnTrue()
    {
        // Arrange
        var value = true;

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void AsBoolean_WithBooleanFalse_ShouldReturnFalse()
    {
        // Arrange
        var value = false;

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void AsBoolean_WithStringTrue_ShouldReturnTrue()
    {
        // Arrange
        var value = "true";

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void AsBoolean_WithStringFalse_ShouldReturnFalse()
    {
        // Arrange
        var value = "false";

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void AsBoolean_WithInvalidString_ShouldReturnFalse()
    {
        // Arrange
        var value = "invalid";

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void AsBoolean_WithNonZeroInt_ShouldReturnTrue()
    {
        // Arrange
        var value = 5;

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void AsBoolean_WithZeroInt_ShouldReturnFalse()
    {
        // Arrange
        var value = 0;

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void AsBoolean_WithNonZeroLong_ShouldReturnTrue()
    {
        // Arrange
        var value = 100L;

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void AsBoolean_WithZeroLong_ShouldReturnFalse()
    {
        // Arrange
        var value = 0L;

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void AsBoolean_WithNonZeroDouble_ShouldReturnTrue()
    {
        // Arrange
        var value = 0.1;

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void AsBoolean_WithZeroDouble_ShouldReturnFalse()
    {
        // Arrange
        var value = 0.0;

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void AsBoolean_WithNonZeroDecimal_ShouldReturnTrue()
    {
        // Arrange
        var value = 1.5m;

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void AsBoolean_WithZeroDecimal_ShouldReturnFalse()
    {
        // Arrange
        var value = 0m;

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void AsBoolean_WithUnsupportedType_ShouldReturnFalse()
    {
        // Arrange
        var value = new object();

        // Act
        var result = value.AsBoolean();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region AsInteger Tests

    [TestMethod]
    public void AsInteger_WithNull_ShouldReturnDefaultValue()
    {
        // Arrange
        object? value = null;

        // Act
        var result = value.AsInteger(42);

        // Assert
        result.Should().Be(42);
    }

    [TestMethod]
    public void AsInteger_WithNullAndNoDefault_ShouldReturnZero()
    {
        // Arrange
        object? value = null;

        // Act
        var result = value.AsInteger();

        // Assert
        result.Should().Be(0);
    }

    [TestMethod]
    public void AsInteger_WithValidInt_ShouldReturnValue()
    {
        // Arrange
        var value = 123;

        // Act
        var result = value.AsInteger();

        // Assert
        result.Should().Be(123);
    }

    [TestMethod]
    public void AsInteger_WithValidString_ShouldReturnParsedValue()
    {
        // Arrange
        var value = "456";

        // Act
        var result = value.AsInteger();

        // Assert
        result.Should().Be(456);
    }

    [TestMethod]
    public void AsInteger_WithInvalidString_ShouldReturnDefaultValue()
    {
        // Arrange
        var value = "invalid";

        // Act
        var result = value.AsInteger(99);

        // Assert
        result.Should().Be(99);
    }

    [TestMethod]
    public void AsInteger_WithDouble_ShouldReturnTruncatedValue()
    {
        // Arrange
        var value = 123.7;

        // Act
        var result = value.AsInteger();

        // Assert
        result.Should().Be(123);
    }

    [TestMethod]
    public void AsInteger_WithDecimal_ShouldReturnTruncatedValue()
    {
        // Arrange
        var value = 456.9m;

        // Act
        var result = value.AsInteger();

        // Assert
        result.Should().Be(456);
    }

    [TestMethod]
    public void AsInteger_WithFloat_ShouldReturnTruncatedValue()
    {
        // Arrange
        var value = 789.3f;

        // Act
        var result = value.AsInteger();

        // Assert
        result.Should().Be(789);
    }

    [TestMethod]
    public void AsInteger_WithBooleanTrue_ShouldReturnOne()
    {
        // Arrange
        var value = true;

        // Act
        var result = value.AsInteger();

        // Assert
        result.Should().Be(1);
    }

    [TestMethod]
    public void AsInteger_WithBooleanFalse_ShouldReturnZero()
    {
        // Arrange
        var value = false;

        // Act
        var result = value.AsInteger();

        // Assert
        result.Should().Be(0);
    }

    [TestMethod]
    public void AsInteger_WithUnsupportedType_ShouldReturnDefaultValue()
    {
        // Arrange
        var value = new object();

        // Act
        var result = value.AsInteger(77);

        // Assert
        result.Should().Be(77);
    }

    #endregion

    #region AsString Tests

    [TestMethod]
    public void AsString_WithNull_ShouldReturnDefaultValue()
    {
        // Arrange
        object? value = null;

        // Act
        var result = value.AsString("default");

        // Assert
        result.Should().Be("default");
    }

    [TestMethod]
    public void AsString_WithNullAndNoDefault_ShouldReturnEmptyString()
    {
        // Arrange
        object? value = null;

        // Act
        var result = value.AsString();

        // Assert
        result.Should().Be("");
    }

    [TestMethod]
    public void AsString_WithString_ShouldReturnSameString()
    {
        // Arrange
        var value = "test string";

        // Act
        var result = value.AsString();

        // Assert
        result.Should().Be("test string");
    }

    [TestMethod]
    public void AsString_WithInteger_ShouldReturnStringRepresentation()
    {
        // Arrange
        var value = 123;

        // Act
        var result = value.AsString();

        // Assert
        result.Should().Be("123");
    }

    [TestMethod]
    public void AsString_WithBoolean_ShouldReturnStringRepresentation()
    {
        // Arrange
        var value = true;

        // Act
        var result = value.AsString();

        // Assert
        result.Should().Be("True");
    }

    [TestMethod]
    public void AsString_WithDateTime_ShouldReturnStringRepresentation()
    {
        // Arrange
        var value = new DateTime(2024, 1, 1);

        // Act
        var result = value.AsString();

        // Assert
        result.Should().Contain("2024");
        result.Should().Contain("01");
    }

    #endregion

    #region AsLong Tests

    [TestMethod]
    public void AsLong_WithNull_ShouldReturnDefaultValue()
    {
        // Arrange
        object? value = null;

        // Act
        var result = value.AsLong(100L);

        // Assert
        result.Should().Be(100L);
    }

    [TestMethod]
    public void AsLong_WithValidLong_ShouldReturnValue()
    {
        // Arrange
        var value = 9876543210L;

        // Act
        var result = value.AsLong();

        // Assert
        result.Should().Be(9876543210L);
    }

    [TestMethod]
    public void AsLong_WithInteger_ShouldReturnLongValue()
    {
        // Arrange
        var value = 123;

        // Act
        var result = value.AsLong();

        // Assert
        result.Should().Be(123L);
    }

    [TestMethod]
    public void AsLong_WithValidString_ShouldReturnParsedValue()
    {
        // Arrange
        var value = "987654321";

        // Act
        var result = value.AsLong();

        // Assert
        result.Should().Be(987654321L);
    }

    [TestMethod]
    public void AsLong_WithInvalidString_ShouldReturnDefaultValue()
    {
        // Arrange
        var value = "invalid";

        // Act
        var result = value.AsLong(555L);

        // Assert
        result.Should().Be(555L);
    }

    #endregion

    #region AsDouble Tests

    [TestMethod]
    public void AsDouble_WithNull_ShouldReturnDefaultValue()
    {
        // Arrange
        object? value = null;

        // Act
        var result = value.AsDouble(1.5);

        // Assert
        result.Should().Be(1.5);
    }

    [TestMethod]
    public void AsDouble_WithValidDouble_ShouldReturnValue()
    {
        // Arrange
        var value = 123.456;

        // Act
        var result = value.AsDouble();

        // Assert
        result.Should().Be(123.456);
    }

    [TestMethod]
    public void AsDouble_WithInteger_ShouldReturnDoubleValue()
    {
        // Arrange
        var value = 42;

        // Act
        var result = value.AsDouble();

        // Assert
        result.Should().Be(42.0);
    }

    [TestMethod]
    public void AsDouble_WithValidString_ShouldReturnParsedValue()
    {
        // Arrange
        var value = "987.654";

        // Act
        var result = value.AsDouble();

        // Assert
        result.Should().Be(987.654);
    }

    [TestMethod]
    public void AsDouble_WithInvalidString_ShouldReturnDefaultValue()
    {
        // Arrange
        var value = "invalid";

        // Act
        var result = value.AsDouble(2.5);

        // Assert
        result.Should().Be(2.5);
    }

    #endregion

    #region AsDateTime Tests

    [TestMethod]
    public void AsDateTime_WithNull_ShouldReturnDefaultValue()
    {
        // Arrange
        object? value = null;
        var defaultValue = new DateTime(2024, 1, 1);

        // Act
        var result = value.AsDateTime(defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [TestMethod]
    public void AsDateTime_WithValidDateTime_ShouldReturnValue()
    {
        // Arrange
        var value = new DateTime(2024, 6, 15);

        // Act
        var result = value.AsDateTime();

        // Assert
        result.Should().Be(new DateTime(2024, 6, 15));
    }

    [TestMethod]
    public void AsDateTime_WithValidString_ShouldReturnParsedValue()
    {
        // Arrange
        var value = "2024-01-01";

        // Act
        var result = value.AsDateTime();

        // Assert
        result.Year.Should().Be(2024);
        result.Month.Should().Be(1);
        result.Day.Should().Be(1);
    }

    [TestMethod]
    public void AsDateTime_WithInvalidString_ShouldReturnDefaultValue()
    {
        // Arrange
        var value = "invalid date";
        var defaultValue = new DateTime(2023, 12, 31);

        // Act
        var result = value.AsDateTime(defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    #endregion

    #region AsGuid Tests

    [TestMethod]
    public void AsGuid_WithNull_ShouldReturnDefaultValue()
    {
        // Arrange
        object? value = null;
        var defaultValue = Guid.NewGuid();

        // Act
        var result = value.AsGuid(defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [TestMethod]
    public void AsGuid_WithValidGuid_ShouldReturnValue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var value = guid;

        // Act
        var result = value.AsGuid();

        // Assert
        result.Should().Be(guid);
    }

    [TestMethod]
    public void AsGuid_WithValidString_ShouldReturnParsedValue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var value = guid.ToString();

        // Act
        var result = value.AsGuid();

        // Assert
        result.Should().Be(guid);
    }

    [TestMethod]
    public void AsGuid_WithInvalidString_ShouldReturnDefaultValue()
    {
        // Arrange
        var value = "invalid-guid";
        var defaultValue = Guid.NewGuid();

        // Act
        var result = value.AsGuid(defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    #endregion

    #region Integration Tests

    [TestMethod]
    public void TypeExtensions_ChainedConversions_ShouldWorkCorrectly()
    {
        // Arrange
        var stringValue = "123";

        // Act
        var asInt = stringValue.AsInteger();
        var asDouble = asInt.AsDouble();
        var asString = asDouble.AsString();
        var asBool = asInt.AsBoolean();

        // Assert
        asInt.Should().Be(123);
        asDouble.Should().Be(123.0);
        asString.Should().Be("123");
        asBool.Should().BeTrue(); // Non-zero int converts to true
    }

    [TestMethod]
    public void TypeExtensions_WithDifferentTypes_ShouldHandleAllScenarios()
    {
        // Test with various input types
        var testCases = new[]
        {
            new { Input = (object)"42", ExpectedInt = 42, ExpectedBool = false }, // String "42" doesn't convert to bool
            new { Input = (object)0, ExpectedInt = 0, ExpectedBool = false },
            new { Input = (object)true, ExpectedInt = 1, ExpectedBool = true },
            new { Input = (object)false, ExpectedInt = 0, ExpectedBool = false },
            new { Input = (object)3.14, ExpectedInt = 3, ExpectedBool = true }
        };

        foreach (var testCase in testCases)
        {
            // Act
            var intResult = testCase.Input.AsInteger();
            var boolResult = testCase.Input.AsBoolean();

            // Assert
            intResult.Should().Be(testCase.ExpectedInt, $"because input {testCase.Input} should convert to {testCase.ExpectedInt}");
            boolResult.Should().Be(testCase.ExpectedBool, $"because input {testCase.Input} should convert to {testCase.ExpectedBool}");
        }
    }

    #endregion
}