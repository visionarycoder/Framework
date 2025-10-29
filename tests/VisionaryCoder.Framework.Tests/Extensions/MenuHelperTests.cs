using FluentAssertions;
using VisionaryCoder.Framework.Extensions.CLI;

namespace VisionaryCoder.Framework.Tests.Extensions;

[TestClass]
public class MenuHelperTests
{
    private StringWriter consoleOutput = null!;
    private StringReader? consoleInput;

    [TestInitialize]
    public void Setup()
    {
        consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
    }

    [TestCleanup]
    public void Cleanup()
    {
        consoleOutput?.Dispose();
        consoleInput?.Dispose();
        
        // Restore original console
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        Console.SetIn(new StreamReader(Console.OpenStandardInput()));
    }

    private void SetConsoleInput(params string[] inputs)
    {
        string inputString = string.Join(Environment.NewLine, inputs);
        consoleInput = new StringReader(inputString);
        Console.SetIn(consoleInput);
    }

    [TestMethod]
    public void ShowIntroduction_WithAppName_ShouldDisplayFormattedIntroduction()
    {
        // Arrange
        string appName = "Test Application";
        int expectedWidth = 72;

        // Act
        MenuHelper.ShowIntroduction(appName);

        // Assert
        string output = consoleOutput.ToString();
        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        lines.Should().HaveCount(5);
        lines[0].Should().Be(new string('-', expectedWidth));
        lines[1].Should().Be("--");
        lines[2].Should().Be($"-- {appName}");
        lines[3].Should().Be("--");
        lines[4].Should().Be(new string('-', expectedWidth));
    }

    [TestMethod]
    public void ShowIntroduction_WithCustomWidth_ShouldDisplayIntroductionWithCustomWidth()
    {
        // Arrange
        string appName = "Custom App";
        int customWidth = 50;

        // Act
        MenuHelper.ShowIntroduction(appName, customWidth);

        // Assert
        string output = consoleOutput.ToString();
        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        lines.Should().HaveCount(5);
        lines[0].Should().Be(new string('-', customWidth));
        lines[1].Should().Be("--");
        lines[2].Should().Be($"-- {appName}");
        lines[3].Should().Be("--");
        lines[4].Should().Be(new string('-', customWidth));
    }

    [TestMethod]
    public void ShowIntroduction_WithEmptyAppName_ShouldDisplayIntroductionWithEmptyName()
    {
        // Arrange
        string appName = "";

        // Act
        MenuHelper.ShowIntroduction(appName);

        // Assert
        string output = consoleOutput.ToString();
        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        lines.Should().HaveCount(5);
        lines[1].Should().Be("--");
        lines[2].Should().Be("-- ");
        lines[3].Should().Be("--");
    }

    [TestMethod]
    public void ShowIntroduction_WithVeryLongAppName_ShouldDisplayIntroductionWithLongName()
    {
        // Arrange
        string appName = "This is a very long application name that exceeds normal length";

        // Act
        MenuHelper.ShowIntroduction(appName);

        // Assert
        string output = consoleOutput.ToString();
        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        lines.Should().HaveCount(5);
        lines[2].Should().Be($"-- {appName}");
    }

    [TestMethod]
    public void ShowIntroduction_WithSpecialCharactersInAppName_ShouldDisplayIntroductionWithSpecialChars()
    {
        // Arrange
        string appName = "App@Name!123#$%";

        // Act
        MenuHelper.ShowIntroduction(appName);

        // Assert
        string output = consoleOutput.ToString();
        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        lines.Should().HaveCount(5);
        lines[2].Should().Be($"-- {appName}");
    }

    [TestMethod]
    public void ShowIntroduction_WithZeroWidth_ShouldDisplayIntroductionWithNoSeparator()
    {
        // Arrange
        string appName = "Test App";
        int zeroWidth = 0;

        // Act
        MenuHelper.ShowIntroduction(appName, zeroWidth);

        // Assert
        string output = consoleOutput.ToString();
        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        lines.Should().HaveCount(3); // Only the -- lines, no separators
        lines[0].Should().Be("--");
        lines[1].Should().Be($"-- {appName}");
        lines[2].Should().Be("--");
    }

    [TestMethod]
    public void ShowIntroduction_WithMinimalWidth_ShouldDisplayIntroductionWithMinimalSeparator()
    {
        // Arrange
        string appName = "App";
        int minimalWidth = 5;

        // Act
        MenuHelper.ShowIntroduction(appName, minimalWidth);

        // Assert
        string output = consoleOutput.ToString();
        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        lines.Should().HaveCount(5);
        lines[0].Should().Be(new string('-', minimalWidth));
        lines[4].Should().Be(new string('-', minimalWidth));
    }

    [TestMethod]
    public void ShowExit_WithDefaultWidth_ShouldDisplayExitMessageAndWaitForInput()
    {
        // Arrange
        SetConsoleInput(""); // Simulate pressing ENTER

        // Act
        MenuHelper.ShowExit();

        // Assert
        string output = consoleOutput.ToString();
        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        lines.Should().HaveCount(3);
        lines[0].Should().Be(new string('-', 72));
        lines[1].Should().Be("Hit [ENTER] to exit.");
        lines[2].Should().Be(new string('-', 72));
    }

    [TestMethod]
    public void ShowExit_WithCustomWidth_ShouldDisplayExitMessageWithDefaultWidthSeparator()
    {
        // Arrange - ShowExit ignores the separateWidth parameter and uses default width for separators
        int customWidth = 40;
        SetConsoleInput(""); // Simulate pressing ENTER

        // Act
        MenuHelper.ShowExit(customWidth);

        // Assert
        string output = consoleOutput.ToString();
        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        lines.Should().HaveCount(3);
        lines[0].Should().Be(new string('-', 72)); // ShowExit always uses default width
        lines[1].Should().Be("Hit [ENTER] to exit.");
        lines[2].Should().Be(new string('-', 72)); // ShowExit always uses default width
    }

    [TestMethod]
    public void ShowExit_ParameterIgnored_DocumentsBugInImplementation()
    {
        // Arrange - This test documents a bug: separateWidth parameter is ignored
        int expectedWidth = 100;
        int actualWidth = 72; // Default width that's actually used
        SetConsoleInput(""); // Simulate pressing ENTER

        // Act
        MenuHelper.ShowExit(expectedWidth);

        // Assert - The parameter is ignored, method uses default width
        string output = consoleOutput.ToString();
        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        lines.Should().HaveCount(3);
        lines[0].Should().Be(new string('-', actualWidth)); // Bug: ignores expectedWidth
        lines[1].Should().Be("Hit [ENTER] to exit.");
        lines[2].Should().Be(new string('-', actualWidth)); // Bug: ignores expectedWidth
        
        // This test documents that ShowExit.separateWidth parameter is not used
        // The method calls ShowSeparator() without parameters, defaulting to width=72
    }

    [TestMethod]
    public void ShowExit_WithZeroWidth_ShouldDisplayExitMessageWithDefaultWidthSeparator()
    {
        // Arrange - ShowExit ignores the separateWidth parameter and uses default width for separators
        SetConsoleInput(""); // Simulate pressing ENTER

        // Act
        MenuHelper.ShowExit(0);

        // Assert
        string output = consoleOutput.ToString();
        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        lines.Should().HaveCount(3); // ShowExit always displays separators with default width
        lines[0].Should().Be(new string('-', 72));
        lines[1].Should().Be("Hit [ENTER] to exit.");
        lines[2].Should().Be(new string('-', 72));
    }

    [TestMethod]
    public void ShowSeparator_WithDefaultWidth_ShouldDisplayDefaultSeparator()
    {
        // Act
        MenuHelper.ShowSeparator();

        // Assert
        string output = consoleOutput.ToString().Trim();
        output.Should().Be(new string('-', 72));
    }

    [TestMethod]
    public void ShowSeparator_WithCustomWidth_ShouldDisplayCustomSeparator()
    {
        // Arrange
        int customWidth = 50;

        // Act
        MenuHelper.ShowSeparator(customWidth);

        // Assert
        string output = consoleOutput.ToString().Trim();
        output.Should().Be(new string('-', customWidth));
    }

    [TestMethod]
    public void ShowSeparator_WithZeroWidth_ShouldDisplayEmptySeparator()
    {
        // Act
        MenuHelper.ShowSeparator(0);

        // Assert
        string output = consoleOutput.ToString().Trim();
        output.Should().Be("");
    }

    [TestMethod]
    public void ShowSeparator_WithNegativeWidth_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Assert - PadRight throws exception for negative values
        var act = () => MenuHelper.ShowSeparator(-5);
        act.Should().Throw<ArgumentOutOfRangeException>()
           .WithParameterName("totalWidth");
    }

    [TestMethod]
    public void ShowSeparator_WithLargeWidth_ShouldDisplayLargeSeparator()
    {
        // Arrange
        int largeWidth = 200;

        // Act
        MenuHelper.ShowSeparator(largeWidth);

        // Assert
        string output = consoleOutput.ToString().Trim();
        output.Should().Be(new string('-', largeWidth));
        output.Length.Should().Be(largeWidth);
    }

    [TestMethod]
    public void ShowSeparator_CalledMultipleTimes_ShouldDisplayMultipleSeparators()
    {
        // Act
        MenuHelper.ShowSeparator(10);
        MenuHelper.ShowSeparator(20);
        MenuHelper.ShowSeparator(5);

        // Assert
        string output = consoleOutput.ToString();
        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        lines.Should().HaveCount(3);
        lines[0].Should().Be(new string('-', 10));
        lines[1].Should().Be(new string('-', 20));
        lines[2].Should().Be(new string('-', 5));
    }

    // Integration tests
    [TestMethod]
    public void MenuHelper_IntegrationTest_ShouldDisplayCompleteMenuFlow()
    {
        // Arrange
        string appName = "Integration Test App";
        SetConsoleInput(""); // For ShowExit

        // Act
        MenuHelper.ShowIntroduction(appName, 50);
        MenuHelper.ShowSeparator(30);
        MenuHelper.ShowExit(50);

        // Assert
        string output = consoleOutput.ToString();
        output.Should().Contain($"-- {appName}");
        output.Should().Contain(new string('-', 50));
        output.Should().Contain(new string('-', 30));
        output.Should().Contain("Hit [ENTER] to exit.");
    }

    [TestMethod]
    public void MenuHelper_ConsistentWidthUsage_ShouldMaintainConsistentFormatting()
    {
        // Arrange
        int width = 80;
        string appName = "Consistent Width Test";
        SetConsoleInput(""); // For ShowExit

        // Act
        MenuHelper.ShowIntroduction(appName, width);
        MenuHelper.ShowSeparator(width);
        MenuHelper.ShowExit(width); // Note: ShowExit ignores the width parameter for separators

        // Assert
        string output = consoleOutput.ToString();
        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        // Count lines with the specified width (80) and default width (72)
        var width80Lines = lines.Where(line => line.Length == 80 && line.All(c => c == '-')).ToList();
        var width72Lines = lines.Where(line => line.Length == 72 && line.All(c => c == '-')).ToList();
        
        width80Lines.Should().HaveCount(3); // 2 from ShowIntroduction, 1 from ShowSeparator
        width72Lines.Should().HaveCount(2); // 2 from ShowExit (which ignores the width parameter)
    }
}