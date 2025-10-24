using FluentAssertions;

namespace VisionaryCoder.Framework.Extensions.Tests;

[TestClass]
public class DivideByZeroExtensionsTests
{
    #region ThrowIfZero Tests

    [TestMethod]
    public void ThrowIfZero_WithZeroInt_ShouldThrowDivideByZeroException()
    {
        // Arrange
        var value = 0;

        // Act & Assert
        var exception = Assert.ThrowsExactly<DivideByZeroException>(() => DivideByZeroExtensions.ThrowIfZero(value));
        exception.Message.Should().Contain("Division by zero would occur");
    }

    [TestMethod]
    public void ThrowIfZero_WithZeroIntAndParamName_ShouldThrowWithParamName()
    {
        // Arrange
        var value = 0;
        var paramName = "divisor";

        // Act & Assert
        var exception = Assert.ThrowsExactly<DivideByZeroException>(() => DivideByZeroExtensions.ThrowIfZero(value, paramName));
        exception.Message.Should().Contain("Division by zero would occur with parameter 'divisor'");
    }

    [TestMethod]
    public void ThrowIfZero_WithNonZeroInt_ShouldNotThrow()
    {
        // Arrange
        var value = 5;

        // Act & Assert
        var action = () => DivideByZeroExtensions.ThrowIfZero(value);
        action.Should().NotThrow();
    }

    [TestMethod]
    public void ThrowIfZero_WithZeroDouble_ShouldThrowDivideByZeroException()
    {
        // Arrange
        var value = 0.0;

        // Act & Assert
        var exception = Assert.ThrowsExactly<DivideByZeroException>(() => DivideByZeroExtensions.ThrowIfZero(value));
        exception.Message.Should().Contain("Division by zero would occur");
    }

    [TestMethod]
    public void ThrowIfZero_WithNonZeroDouble_ShouldNotThrow()
    {
        // Arrange
        var value = 3.14;

        // Act & Assert
        var action = () => DivideByZeroExtensions.ThrowIfZero(value);
        action.Should().NotThrow();
    }

    [TestMethod]
    public void ThrowIfZero_WithZeroDecimal_ShouldThrowDivideByZeroException()
    {
        // Arrange
        var value = 0m;

        // Act & Assert
        var exception = Assert.ThrowsExactly<DivideByZeroException>(() => DivideByZeroExtensions.ThrowIfZero(value));
        exception.Message.Should().Contain("Division by zero would occur");
    }

    [TestMethod]
    public void ThrowIfZero_WithNonZeroDecimal_ShouldNotThrow()
    {
        // Arrange
        var value = 1.5m;

        // Act & Assert
        var action = () => DivideByZeroExtensions.ThrowIfZero(value);
        action.Should().NotThrow();
    }

    #endregion

    #region IsZero Tests

    [TestMethod]
    public void IsZero_WithZeroInt_ShouldReturnTrue()
    {
        // Arrange
        var value = 0;

        // Act
        var result = value.IsZero();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void IsZero_WithNonZeroInt_ShouldReturnFalse()
    {
        // Arrange
        var value = 42;

        // Act
        var result = value.IsZero();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void IsZero_WithZeroDouble_ShouldReturnTrue()
    {
        // Arrange
        var value = 0.0;

        // Act
        var result = value.IsZero();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void IsZero_WithNonZeroDouble_ShouldReturnFalse()
    {
        // Arrange
        var value = 1.23;

        // Act
        var result = value.IsZero();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void IsZero_WithZeroDecimal_ShouldReturnTrue()
    {
        // Arrange
        var value = 0m;

        // Act
        var result = value.IsZero();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void IsZero_WithNonZeroDecimal_ShouldReturnFalse()
    {
        // Arrange
        var value = 5.67m;

        // Act
        var result = value.IsZero();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void IsZero_WithZeroFloat_ShouldReturnTrue()
    {
        // Arrange
        var value = 0.0f;

        // Act
        var result = value.IsZero();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void IsZero_WithNonZeroFloat_ShouldReturnFalse()
    {
        // Arrange
        var value = 2.5f;

        // Act
        var result = value.IsZero();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region SafeDivide Tests (with default value)

    [TestMethod]
    public void SafeDivide_WithNonZeroDenominator_ShouldReturnQuotient()
    {
        // Arrange
        var numerator = 10;
        var denominator = 2;
        var defaultValue = 999;

        // Act
        var result = DivideByZeroExtensions.SafeDivide(numerator, denominator, defaultValue);

        // Assert
        result.Should().Be(5);
    }

    [TestMethod]
    public void SafeDivide_WithZeroDenominator_ShouldReturnDefaultValue()
    {
        // Arrange
        var numerator = 10;
        var denominator = 0;
        var defaultValue = 999;

        // Act
        var result = DivideByZeroExtensions.SafeDivide(numerator, denominator, defaultValue);

        // Assert
        result.Should().Be(999);
    }

    [TestMethod]
    public void SafeDivide_WithDoubles_ShouldWorkCorrectly()
    {
        // Arrange
        var numerator = 15.0;
        var denominator = 3.0;
        var defaultValue = -1.0;

        // Act
        var result = DivideByZeroExtensions.SafeDivide(numerator, denominator, defaultValue);

        // Assert
        result.Should().Be(5.0);
    }

    [TestMethod]
    public void SafeDivide_WithZeroDoublesDenominator_ShouldReturnDefaultValue()
    {
        // Arrange
        var numerator = 15.0;
        var denominator = 0.0;
        var defaultValue = -1.0;

        // Act
        var result = DivideByZeroExtensions.SafeDivide(numerator, denominator, defaultValue);

        // Assert
        result.Should().Be(-1.0);
    }

    [TestMethod]
    public void SafeDivide_WithDecimals_ShouldWorkCorrectly()
    {
        // Arrange
        var numerator = 20.5m;
        var denominator = 4.1m;
        var defaultValue = 0m;

        // Act
        var result = DivideByZeroExtensions.SafeDivide(numerator, denominator, defaultValue);

        // Assert
        result.Should().Be(5m);
    }

    #endregion

    #region SafeDivide Tests (without default value - returns zero)

    [TestMethod]
    public void SafeDivide_WithoutDefault_WithNonZeroDenominator_ShouldReturnQuotient()
    {
        // Arrange
        var numerator = 20;
        var denominator = 4;

        // Act
        var result = DivideByZeroExtensions.SafeDivide(numerator, denominator);

        // Assert
        result.Should().Be(5);
    }

    [TestMethod]
    public void SafeDivide_WithoutDefault_WithZeroDenominator_ShouldReturnZero()
    {
        // Arrange
        var numerator = 10;
        var denominator = 0;

        // Act
        var result = DivideByZeroExtensions.SafeDivide(numerator, denominator);

        // Assert
        result.Should().Be(0);
    }

    [TestMethod]
    public void SafeDivide_WithoutDefault_WithDoubles_ShouldWorkCorrectly()
    {
        // Arrange
        var numerator = 12.0;
        var denominator = 0.0;

        // Act
        var result = DivideByZeroExtensions.SafeDivide(numerator, denominator);

        // Assert
        result.Should().Be(0.0);
    }

    #endregion

    #region TryDivide Tests

    [TestMethod]
    public void TryDivide_WithNonZeroDenominator_ShouldReturnTrueAndCorrectResult()
    {
        // Arrange
        var numerator = 15;
        var denominator = 3;

        // Act
        var success = DivideByZeroExtensions.TryDivide(numerator, denominator, out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(5);
    }

    [TestMethod]
    public void TryDivide_WithZeroDenominator_ShouldReturnFalseAndDefaultResult()
    {
        // Arrange
        var numerator = 10;
        var denominator = 0;

        // Act
        var success = DivideByZeroExtensions.TryDivide(numerator, denominator, out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(0); // default for int
    }

    [TestMethod]
    public void TryDivide_WithDoubles_ShouldWorkCorrectly()
    {
        // Arrange
        var numerator = 21.0;
        var denominator = 7.0;

        // Act
        var success = DivideByZeroExtensions.TryDivide(numerator, denominator, out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(3.0);
    }

    [TestMethod]
    public void TryDivide_WithZeroDoubleDenominator_ShouldReturnFalse()
    {
        // Arrange
        var numerator = 10.5;
        var denominator = 0.0;

        // Act
        var success = DivideByZeroExtensions.TryDivide(numerator, denominator, out var result);

        // Assert
        success.Should().BeFalse();
        result.Should().Be(0.0);
    }

    [TestMethod]
    public void TryDivide_WithDecimals_ShouldWorkCorrectly()
    {
        // Arrange
        var numerator = 24.6m;
        var denominator = 6.15m;

        // Act
        var success = DivideByZeroExtensions.TryDivide(numerator, denominator, out var result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(4m);
    }

    #endregion

    #region DefaultIfZero Tests

    [TestMethod]
    public void DefaultIfZero_WithZeroValue_ShouldReturnDefault()
    {
        // Arrange
        var value = 0;
        var defaultValue = 42;

        // Act
        var result = value.DefaultIfZero(defaultValue);

        // Assert
        result.Should().Be(42);
    }

    [TestMethod]
    public void DefaultIfZero_WithNonZeroValue_ShouldReturnOriginalValue()
    {
        // Arrange
        var value = 15;
        var defaultValue = 42;

        // Act
        var result = value.DefaultIfZero(defaultValue);

        // Assert
        result.Should().Be(15);
    }

    [TestMethod]
    public void DefaultIfZero_WithZeroDouble_ShouldReturnDefault()
    {
        // Arrange
        var value = 0.0;
        var defaultValue = 3.14;

        // Act
        var result = value.DefaultIfZero(defaultValue);

        // Assert
        result.Should().Be(3.14);
    }

    [TestMethod]
    public void DefaultIfZero_WithNonZeroDouble_ShouldReturnOriginalValue()
    {
        // Arrange
        var value = 2.5;
        var defaultValue = 3.14;

        // Act
        var result = value.DefaultIfZero(defaultValue);

        // Assert
        result.Should().Be(2.5);
    }

    [TestMethod]
    public void DefaultIfZero_WithZeroDecimal_ShouldReturnDefault()
    {
        // Arrange
        var value = 0m;
        var defaultValue = 9.99m;

        // Act
        var result = value.DefaultIfZero(defaultValue);

        // Assert
        result.Should().Be(9.99m);
    }

    [TestMethod]
    public void DefaultIfZero_WithNonZeroDecimal_ShouldReturnOriginalValue()
    {
        // Arrange
        var value = 7.77m;
        var defaultValue = 9.99m;

        // Act
        var result = value.DefaultIfZero(defaultValue);

        // Assert
        result.Should().Be(7.77m);
    }

    #endregion

    #region Integration Tests

    [TestMethod]
    public void DivideByZeroExtensions_ComplexScenario_ShouldWorkCorrectly()
    {
        // Arrange
        var values = new[] { 10, 0, 5, 20 };
        var divisor = 2;
        var results = new List<int>();

        // Act
        foreach (var value in values)
        {
            if (!value.IsZero())
            {
                DivideByZeroExtensions.ThrowIfZero(divisor); // This should not throw for divisor = 2
                var result = DivideByZeroExtensions.SafeDivide(value, divisor);
                results.Add(result);
            }
            else
            {
                results.Add(value.DefaultIfZero(99));
            }
        }

        // Assert
        results.Should().ContainInOrder(5, 99, 2, 10); // 10/2=5, 0->99, 5/2=2, 20/2=10
    }

    [TestMethod]
    public void DivideByZeroExtensions_WithDifferentNumericTypes_ShouldWorkConsistently()
    {
        // Test with int
        var intResult = DivideByZeroExtensions.SafeDivide(10, 0, -1);
        intResult.Should().Be(-1);

        // Test with double  
        var doubleResult = DivideByZeroExtensions.SafeDivide(10.0, 0.0, -1.0);
        doubleResult.Should().Be(-1.0);

        // Test with decimal
        var decimalResult = DivideByZeroExtensions.SafeDivide(10m, 0m, -1m);
        decimalResult.Should().Be(-1m);

        // Test with float
        var floatResult = DivideByZeroExtensions.SafeDivide(10f, 0f, -1f);
        floatResult.Should().Be(-1f);

        // All should consistently return the default value when dividing by zero
        intResult.Should().Be(-1);
        doubleResult.Should().Be(-1.0);
        decimalResult.Should().Be(-1m);
        floatResult.Should().Be(-1f);
    }

    [TestMethod]
    public void DivideByZeroExtensions_TryDividePattern_ShouldHandleEdgeCases()
    {
        // Test successful division
        var success1 = DivideByZeroExtensions.TryDivide(100, 25, out var result1);
        success1.Should().BeTrue();
        result1.Should().Be(4);

        // Test zero division
        var success2 = DivideByZeroExtensions.TryDivide(100, 0, out var result2);
        success2.Should().BeFalse();
        result2.Should().Be(0);

        // Test zero numerator (valid division)
        var success3 = DivideByZeroExtensions.TryDivide(0, 5, out var result3);
        success3.Should().BeTrue();
        result3.Should().Be(0);
    }

    #endregion
}