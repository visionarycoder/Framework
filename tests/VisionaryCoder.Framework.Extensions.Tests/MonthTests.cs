using FluentAssertions;

namespace VisionaryCoder.Framework.Extensions.Tests;

/// <summary>
/// Unit tests for the Month class to ensure 100% code coverage.
/// </summary>
[TestClass]
public class MonthTests
{
    #region Constructor Tests

    [TestMethod]
    public void DefaultConstructor_ShouldCreateUnknownMonth()
    {
        // Arrange & Act
        var month = new Month();

        // Assert
        month.Name.Should().Be(Month.Unknown);
        month.Order.Should().Be(0);
        month.Index.Should().Be(-1);
        month.Abbrv.Should().Be("???");
    }

    [TestMethod]
    [DataRow(0, Month.Unknown)]
    [DataRow(1, Month.January)]
    [DataRow(2, Month.February)]
    [DataRow(3, Month.March)]
    [DataRow(4, Month.April)]
    [DataRow(5, Month.May)]
    [DataRow(6, Month.June)]
    [DataRow(7, Month.July)]
    [DataRow(8, Month.August)]
    [DataRow(9, Month.September)]
    [DataRow(10, Month.October)]
    [DataRow(11, Month.November)]
    [DataRow(12, Month.December)]
    public void ConstructorWithValidOrder_ShouldCreateCorrectMonth(int order, string expectedName)
    {
        // Arrange & Act
        var month = new Month(order);

        // Assert
        month.Order.Should().Be(order);
        month.Name.Should().Be(expectedName);
        month.Index.Should().Be(order - 1);
        month.Abbrv.Should().Be(expectedName[..3]);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(13)]
    [DataRow(100)]
    public void ConstructorWithInvalidOrder_ShouldThrowArgumentOutOfRangeException(int invalidOrder)
    {
        // Arrange & Act & Assert
        var action = () => new Month(invalidOrder);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("order")
            .WithMessage("Order must be between 0 and 13*");
    }

    [TestMethod]
    [DataRow(Month.Unknown, 0)]
    [DataRow(Month.January, 1)]
    [DataRow(Month.February, 2)]
    [DataRow(Month.March, 3)]
    [DataRow(Month.April, 4)]
    [DataRow(Month.May, 5)]
    [DataRow(Month.June, 6)]
    [DataRow(Month.July, 7)]
    [DataRow(Month.August, 8)]
    [DataRow(Month.September, 9)]
    [DataRow(Month.October, 10)]
    [DataRow(Month.November, 11)]
    [DataRow(Month.December, 12)]
    public void ConstructorWithValidLongName_ShouldCreateCorrectMonth(string name, int expectedOrder)
    {
        // Arrange & Act
        var month = new Month(name);

        // Assert
        month.Name.Should().Be(name);
        month.Order.Should().Be(expectedOrder);
        month.Index.Should().Be(expectedOrder - 1);
    }

    [TestMethod]
    [DataRow(Month.Jan, Month.January, 1)]
    [DataRow(Month.Feb, Month.February, 2)]
    [DataRow(Month.Mar, Month.March, 3)]
    [DataRow(Month.Apr, Month.April, 4)]
    [DataRow("May", Month.May, 5)] // May is same in short and long form
    [DataRow(Month.Jun, Month.June, 6)]
    [DataRow(Month.Jul, Month.July, 7)]
    [DataRow(Month.Aug, Month.August, 8)]
    [DataRow(Month.Sep, Month.September, 9)]
    [DataRow(Month.Oct, Month.October, 10)]
    [DataRow(Month.Nov, Month.November, 11)]
    [DataRow(Month.Dec, Month.December, 12)]
    public void ConstructorWithValidShortName_ShouldCreateCorrectMonth(string shortName, string expectedLongName, int expectedOrder)
    {
        // Arrange & Act
        var month = new Month(shortName);

        // Assert
        month.Name.Should().Be(expectedLongName);
        month.Order.Should().Be(expectedOrder);
        month.Index.Should().Be(expectedOrder - 1);
    }

    [TestMethod]
    public void ConstructorWithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new Month(null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("name");
    }

    [TestMethod]
    [DataRow("InvalidMonth")]
    [DataRow("")]
    [DataRow("13th")]
    [DataRow("NotAMonth")]
    public void ConstructorWithInvalidName_ShouldThrowArgumentOutOfRangeException(string invalidName)
    {
        // Arrange & Act & Assert
        var action = () => new Month(invalidName);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("name")
            .WithMessage($"Name is not a valid month name: {invalidName}*");
    }

    #endregion

    #region Property Tests

    [TestMethod]
    public void Name_ShouldReturnCorrectValue()
    {
        // Arrange
        var month = new Month(Month.January);

        // Act & Assert
        month.Name.Should().Be(Month.January);
    }

    [TestMethod]
    public void Order_ShouldReturnCorrectValue()
    {
        // Arrange
        var month = new Month(5);

        // Act & Assert
        month.Order.Should().Be(5);
    }

    [TestMethod]
    public void Index_ShouldReturnOrderMinusOne()
    {
        // Arrange
        var month = new Month(5);

        // Act & Assert
        month.Index.Should().Be(4);
    }

    [TestMethod]
    public void Abbrv_ShouldReturnFirstThreeCharacters()
    {
        // Arrange
        var month = new Month(Month.January);

        // Act & Assert
        month.Abbrv.Should().Be("Jan");
    }

    [TestMethod]
    public void Abbrv_ForUnknown_ShouldReturnThreeQuestionMarks()
    {
        // Arrange
        var month = new Month();

        // Act & Assert
        month.Abbrv.Should().Be("???");
    }

    #endregion

    #region Method Tests

    [TestMethod]
    public void ToString_ShouldReturnName()
    {
        // Arrange
        var month = new Month(Month.March);

        // Act
        var result = month.ToString();

        // Assert
        result.Should().Be(Month.March);
    }

    [TestMethod]
    public void ToString_ForUnknown_ShouldReturnUnknown()
    {
        // Arrange
        var month = new Month();

        // Act
        var result = month.ToString();

        // Assert
        result.Should().Be(Month.Unknown);
    }

    #endregion

    #region Constant Tests

    [TestMethod]
    public void Constants_ShouldHaveCorrectValues()
    {
        // Assert - Test all month constants
        Month.Unknown.Should().Be("???");
        Month.January.Should().Be("January");
        Month.February.Should().Be("February");
        Month.March.Should().Be("March");
        Month.April.Should().Be("April");
        Month.May.Should().Be("May");
        Month.June.Should().Be("June");
        Month.July.Should().Be("July");
        Month.August.Should().Be("August");
        Month.September.Should().Be("September");
        Month.October.Should().Be("October");
        Month.November.Should().Be("November");
        Month.December.Should().Be("December");

        // Short month constants
        Month.Jan.Should().Be("Jan");
        Month.Feb.Should().Be("Feb");
        Month.Mar.Should().Be("Mar");
        Month.Apr.Should().Be("Apr");
        Month.Jun.Should().Be("Jun");
        Month.Jul.Should().Be("Jul");
        Month.Aug.Should().Be("Aug");
        Month.Sep.Should().Be("Sep");
        Month.Oct.Should().Be("Oct");
        Month.Nov.Should().Be("Nov");
        Month.Dec.Should().Be("Dec");
    }

    #endregion

    #region Edge Case Tests

    [TestMethod]
    public void Constructor_WithBoundaryValues_ShouldWorkCorrectly()
    {
        // Test first valid value
        var firstMonth = new Month(0);
        firstMonth.Name.Should().Be(Month.Unknown);
        firstMonth.Order.Should().Be(0);

        // Test last valid value
        var lastMonth = new Month(12);
        lastMonth.Name.Should().Be(Month.December);
        lastMonth.Order.Should().Be(12);
    }

    [TestMethod]
    public void Constructor_WithCaseSensitiveNames_ShouldThrowForIncorrectCase()
    {
        // Arrange & Act & Assert
        var action = () => new Month("january"); // lowercase
        action.Should().Throw<ArgumentOutOfRangeException>();

        var action2 = () => new Month("JANUARY"); // uppercase
        action2.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion
}