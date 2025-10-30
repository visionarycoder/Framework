using FluentAssertions;
using VisionaryCoder.Framework.Extensions.CLI;

namespace VisionaryCoder.Framework.Tests.Extensions;

[TestClass]
public class CliInputUtilitiesTests
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
    public void GetDecimalInput_WithValidInput_ShouldReturnDecimal()
    {
        // Arrange
        SetConsoleInput("123.45");

        // Act
        decimal result = CliInputUtilities.GetDecimalInput();

        // Assert
        result.Should().Be(123.45m);
    }

    [TestMethod]
    public void GetDecimalInput_WithInvalidThenValidInput_ShouldReturnDecimalAfterErrorMessage()
    {
        // Arrange
        SetConsoleInput("invalid", "456.78");

        // Act
        decimal result = CliInputUtilities.GetDecimalInput();

        // Assert
        result.Should().Be(456.78m);
        consoleOutput.ToString().Should().Contain("Invalid input.  Please try again.");
    }

    [TestMethod]
    public void GetDecimalInput_WithWhitespaceAroundValidInput_ShouldReturnDecimal()
    {
        // Arrange
        SetConsoleInput("  789.12  ");

        // Act
        decimal result = CliInputUtilities.GetDecimalInput();

        // Assert
        result.Should().Be(789.12m);
    }

    [TestMethod]
    public void GetDecimalInput_WithZero_ShouldReturnZero()
    {
        // Arrange
        SetConsoleInput("0");

        // Act
        decimal result = CliInputUtilities.GetDecimalInput();

        // Assert
        result.Should().Be(0m);
    }

    [TestMethod]
    public void GetDecimalInput_WithNegativeNumber_ShouldReturnNegativeDecimal()
    {
        // Arrange
        SetConsoleInput("-123.45");

        // Act
        decimal result = CliInputUtilities.GetDecimalInput();

        // Assert
        result.Should().Be(-123.45m);
    }

    [TestMethod]
    public void GetIntegerInput_WithValidInput_ShouldReturnInteger()
    {
        // Arrange
        SetConsoleInput("42");

        // Act
        int result = CliInputUtilities.GetIntegerInput();

        // Assert
        result.Should().Be(42);
    }

    [TestMethod]
    public void GetIntegerInput_WithInvalidThenValidInput_ShouldReturnIntegerAfterErrorMessage()
    {
        // Arrange
        SetConsoleInput("invalid", "123");

        // Act
        int result = CliInputUtilities.GetIntegerInput();

        // Assert
        result.Should().Be(123);
        consoleOutput.ToString().Should().Contain("Invalid input.  Please try again.");
    }

    [TestMethod]
    public void GetIntegerInput_WithWhitespaceAroundValidInput_ShouldReturnInteger()
    {
        // Arrange
        SetConsoleInput("  999  ");

        // Act
        int result = CliInputUtilities.GetIntegerInput();

        // Assert
        result.Should().Be(999);
    }

    [TestMethod]
    public void GetIntegerInput_WithZero_ShouldReturnZero()
    {
        // Arrange
        SetConsoleInput("0");

        // Act
        int result = CliInputUtilities.GetIntegerInput();

        // Assert
        result.Should().Be(0);
    }

    [TestMethod]
    public void GetIntegerInput_WithNegativeNumber_ShouldReturnNegativeInteger()
    {
        // Arrange
        SetConsoleInput("-42");

        // Act
        int result = CliInputUtilities.GetIntegerInput();

        // Assert
        result.Should().Be(-42);
    }

    [TestMethod]
    public void GetIntegerInput_WithDecimalInput_ShouldShowErrorAndRetryUntilValidInteger()
    {
        // Arrange
        SetConsoleInput("123.45", "100");

        // Act
        int result = CliInputUtilities.GetIntegerInput();

        // Assert
        result.Should().Be(100);
        consoleOutput.ToString().Should().Contain("Invalid input.  Please try again.");
    }

    [TestMethod]
    public void GetStringInput_WithValidInput_ShouldReturnUppercaseString()
    {
        // Arrange
        SetConsoleInput("hello world");

        // Act
        string result = CliInputUtilities.GetStringInput();

        // Assert
        result.Should().Be("HELLO WORLD");
    }

    [TestMethod]
    public void GetStringInput_WithWhitespaceAroundInput_ShouldReturnTrimmedUppercaseString()
    {
        // Arrange
        SetConsoleInput("  test  ");

        // Act
        string result = CliInputUtilities.GetStringInput();

        // Assert
        result.Should().Be("TEST");
    }

    [TestMethod]
    public void GetStringInput_WithEmptyThenValidInput_ShouldReturnStringAfterErrorMessage()
    {
        // Arrange
        SetConsoleInput("", "valid");

        // Act
        string result = CliInputUtilities.GetStringInput();

        // Assert
        result.Should().Be("VALID");
        consoleOutput.ToString().Should().Contain("Invalid input.  Please try again.");
    }

    [TestMethod]
    public void GetStringInput_WithWhitespaceOnlyThenValidInput_ShouldReturnStringAfterErrorMessage()
    {
        // Arrange
        SetConsoleInput("   ", "test");

        // Act
        string result = CliInputUtilities.GetStringInput();

        // Assert
        result.Should().Be("TEST");
        consoleOutput.ToString().Should().Contain("Invalid input.  Please try again.");
    }

    [TestMethod]
    public void GetStringInput_WithMixedCaseInput_ShouldReturnUppercaseString()
    {
        // Arrange
        SetConsoleInput("MiXeD cAsE");

        // Act
        string result = CliInputUtilities.GetStringInput();

        // Assert
        result.Should().Be("MIXED CASE");
    }

    [TestMethod]
    public void PromptForInputFile_WithValidFilePath_ShouldReturnFileInfo()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            SetConsoleInput(tempFile);

            // Act
            FileInfo? result = CliInputUtilities.PromptForInputFile();

            // Assert
            result.Should().NotBeNull();
            result!.FullName.Should().Be(tempFile);
            consoleOutput.ToString().Should().Contain("Please enter the path to your file (or type 'exit' to quit):");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void PromptForInputFile_WithNonExistentFile_ShouldShowErrorAndRetry()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        string nonExistentFile = Path.Combine(Path.GetTempPath(), "nonexistent.txt");
        try
        {
            SetConsoleInput(nonExistentFile, tempFile);

            // Act
            FileInfo? result = CliInputUtilities.PromptForInputFile();

            // Assert
            result.Should().NotBeNull();
            result!.FullName.Should().Be(tempFile);
            consoleOutput.ToString().Should().Contain("File does not exist.");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void PromptForInputFile_WithEmptyInput_ShouldShowErrorAndRetry()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            SetConsoleInput("", tempFile);

            // Act
            FileInfo? result = CliInputUtilities.PromptForInputFile();

            // Assert
            result.Should().NotBeNull();
            result!.FullName.Should().Be(tempFile);
            consoleOutput.ToString().Should().Contain("File path cannot be empty.");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void PromptForInputFile_WithExitCommand_ShouldReturnNull()
    {
        // Arrange
        SetConsoleInput("exit");

        // Act
        FileInfo? result = CliInputUtilities.PromptForInputFile();

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void PromptForInputFile_WithXCommand_ShouldReturnNull()
    {
        // Arrange
        SetConsoleInput("x");

        // Act
        FileInfo? result = CliInputUtilities.PromptForInputFile();

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void PromptForInputFile_WithQCommand_ShouldReturnNull()
    {
        // Arrange
        SetConsoleInput("q");

        // Act
        FileInfo? result = CliInputUtilities.PromptForInputFile();

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void PromptForInputFile_WithUppercaseExitCommand_ShouldReturnNull()
    {
        // Arrange
        SetConsoleInput("EXIT");

        // Act
        FileInfo? result = CliInputUtilities.PromptForInputFile();

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void PromptForInputFolder_WithValidFolderPath_ShouldReturnDirectoryInfo()
    {
        // Arrange
        string tempFolder = Path.GetTempPath();
        SetConsoleInput(tempFolder);

        // Act
        DirectoryInfo? result = CliInputUtilities.PromptForInputFolder();

        // Assert
        result.Should().NotBeNull();
        result!.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Should().Be(tempFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        consoleOutput.ToString().Should().Contain("Please enter the path to folder (or x|q|exit to return to the previous menu):");
    }

    [TestMethod]
    public void PromptForInputFolder_WithNonExistentFolder_ShouldShowErrorAndRetry()
    {
        // Arrange
        string tempFolder = Path.GetTempPath();
        string nonExistentFolder = Path.Combine(Path.GetTempPath(), "nonexistent");
        SetConsoleInput(nonExistentFolder, tempFolder);

        // Act
        DirectoryInfo? result = CliInputUtilities.PromptForInputFolder();

        // Assert
        result.Should().NotBeNull();
        consoleOutput.ToString().Should().Contain("Folder does not exist.");
    }

    [TestMethod]
    public void PromptForInputFolder_WithEmptyInput_ShouldShowErrorAndRetry()
    {
        // Arrange
        string tempFolder = Path.GetTempPath();
        SetConsoleInput("", tempFolder);

        // Act
        DirectoryInfo? result = CliInputUtilities.PromptForInputFolder();

        // Assert
        result.Should().NotBeNull();
        consoleOutput.ToString().Should().Contain("Input Error: Input cannot be empty.");
    }

    [TestMethod]
    public void PromptForInputFolder_WithExitCommand_ShouldReturnNull()
    {
        // Arrange
        SetConsoleInput("exit");

        // Act
        DirectoryInfo? result = CliInputUtilities.PromptForInputFolder();

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void PromptForInputFolder_WithXCommand_ShouldReturnNull()
    {
        // Arrange
        SetConsoleInput("x");

        // Act
        DirectoryInfo? result = CliInputUtilities.PromptForInputFolder();

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void PromptForInputFolder_WithQCommand_ShouldReturnNull()
    {
        // Arrange
        SetConsoleInput("q");

        // Act
        DirectoryInfo? result = CliInputUtilities.PromptForInputFolder();

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void PromptForInputFolder_WithUppercaseExitCommand_ShouldReturnNull()
    {
        // Arrange
        SetConsoleInput("EXIT");

        // Act
        DirectoryInfo? result = CliInputUtilities.PromptForInputFolder();

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void PromptForInputFolder_WithWhitespaceAroundPath_ShouldTrimAndValidate()
    {
        // Arrange
        string tempFolder = Path.GetTempPath();
        SetConsoleInput($"  {tempFolder}  ");

        // Act
        DirectoryInfo? result = CliInputUtilities.PromptForInputFolder();

        // Assert
        result.Should().NotBeNull();
        result!.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Should().Be(tempFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }

    // Testing edge cases for decimal parsing
    [TestMethod]
    public void GetDecimalInput_WithMaxValue_ShouldReturnMaxDecimal()
    {
        // Arrange
        SetConsoleInput(decimal.MaxValue.ToString());

        // Act
        decimal result = CliInputUtilities.GetDecimalInput();

        // Assert
        result.Should().Be(decimal.MaxValue);
    }

    [TestMethod]
    public void GetDecimalInput_WithMinValue_ShouldReturnMinDecimal()
    {
        // Arrange
        SetConsoleInput(decimal.MinValue.ToString());

        // Act
        decimal result = CliInputUtilities.GetDecimalInput();

        // Assert
        result.Should().Be(decimal.MinValue);
    }

    // Testing edge cases for integer parsing
    [TestMethod]
    public void GetIntegerInput_WithMaxValue_ShouldReturnMaxInteger()
    {
        // Arrange
        SetConsoleInput(int.MaxValue.ToString());

        // Act
        int result = CliInputUtilities.GetIntegerInput();

        // Assert
        result.Should().Be(int.MaxValue);
    }

    [TestMethod]
    public void GetIntegerInput_WithMinValue_ShouldReturnMinInteger()
    {
        // Arrange
        SetConsoleInput(int.MinValue.ToString());

        // Act
        int result = CliInputUtilities.GetIntegerInput();

        // Assert
        result.Should().Be(int.MinValue);
    }

    [TestMethod]
    public void GetStringInput_WithSpecialCharacters_ShouldReturnUppercaseString()
    {
        // Arrange
        SetConsoleInput("hello@world!123");

        // Act
        string result = CliInputUtilities.GetStringInput();

        // Assert
        result.Should().Be("HELLO@WORLD!123");
    }

    [TestMethod]
    public void GetStringInput_WithUnicodeCharacters_ShouldReturnUppercaseString()
    {
        // Arrange
        SetConsoleInput("héllo wörld");

        // Act
        string result = CliInputUtilities.GetStringInput();

        // Assert
        result.Should().Be("HÉLLO WÖRLD");
    }
}
