using System.Globalization;

namespace VisionaryCoder.Framework.Extensions;
public static class CliInputUtilities
{
    public const string InvalidInputMessage = "Invalid input.  Please try again.";
    public const string FilePromptMessage = "Please enter the path to your file (or type 'exit' to quit):";
    public const string FileEmptyErrorMessage = "File path cannot be empty.";
    public const string FileNotExistErrorMessage = "File does not exist.";
    public const string FolderPromptMessage = "Please enter the path to folder (or x|q|exit to return to the previous menu):";
    public const string FolderEmptyErrorMessage = "Input Error: Input cannot be empty.";
    public const string FolderNotExistErrorMessage = "Folder does not exist.";

    public static decimal GetDecimalInput()
    {
        do
        {
            var trimmedInput = GetTrimmedInput();
            if (decimal.TryParse(trimmedInput, out var value))
            {
                return value;
            }
            Console.WriteLine(InvalidInputMessage);
        } while (true);
    }
    
    public static int GetIntegerInput()
    {
        do
        {
            var trimmedInput = GetTrimmedInput();
            if (int.TryParse(trimmedInput, out var value))
            {
                return value;
            }
            Console.WriteLine(InvalidInputMessage);
        } while (true);
    }

    public static string GetStringInput()
    {
        var trimmedInput = GetTrimmedInput()?.ToUpperInvariant();
        if (!string.IsNullOrWhiteSpace(trimmedInput))
        {
            return trimmedInput;
        }
        Console.WriteLine(InvalidInputMessage);
        return string.Empty;
    }

    public static FileInfo? PromptForInputFile()
    {
        return PromptForPath(FilePromptMessage, FileEmptyErrorMessage, FileNotExistErrorMessage, path => new FileInfo(path).Exists ? new FileInfo(path) : null);
    }

    public static DirectoryInfo? PromptForInputFolder()
    {
        return PromptForPath(FolderPromptMessage, FolderEmptyErrorMessage, FolderNotExistErrorMessage, path => new DirectoryInfo(path).Exists ? new DirectoryInfo(path) : null);
    }

    private static string? GetTrimmedInput()
    {
        var rawInput = Console.ReadLine();
        return rawInput?.Trim();
    }

    private static T? PromptForPath<T>(string promptMessage, string emptyErrorMessage, string notExistErrorMessage, Func<string, T?> getPathInfoFunc) where T : class
    {
        while (true)
        {
            Console.WriteLine(promptMessage);
            var path = GetTrimmedInput();
            if (IsNullOrEmpty(path))
            {
                Console.WriteLine(emptyErrorMessage);
                continue;
            }
            if (IsExitCommand(path!))
            {
                return null;
            }
            var pathInfo = getPathInfoFunc(path!);
            if (pathInfo != null)
            {
                return pathInfo;
            }
            Console.WriteLine(notExistErrorMessage);
        }
    }

    private static bool IsExitCommand(string input)
    {
        return input.ToLower(CultureInfo.CurrentCulture) is "exit" or "x" or "q";
    }

    private static bool IsNullOrEmpty(string? input)
    {
        return string.IsNullOrEmpty(input);
    }

}
