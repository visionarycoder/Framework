// ReSharper disable ClassNeverInstantiated.Global

using vc.Ifx.Services;

namespace v9.Ifx.Services.OS.Windows.Cli.Menu;

public class MenuService: ServiceBase, IMenuService
{
    /// <summary>
    /// Prompts the user for an integer input with validation.
    /// </summary>
    /// <param name="promptText">The text to display as a prompt.</param>
    /// <param name="defaultValue">Optional default value to use if the user presses Enter.</param>
    /// <param name="validator">Optional validation function.</param>
    /// <returns>The validated integer input.</returns>
    public int PromptForInteger(string promptText, int? defaultValue = null, Func<int, bool>? validator = null)
    {
        var defaultDisplay = defaultValue.HasValue ? $" [{defaultValue}]" : "";
        Console.Write($"{promptText}{defaultDisplay}: ");
        
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) && defaultValue.HasValue)
        {
            return defaultValue.Value;
        }
        
        int value;
        while (!int.TryParse(input, out value) || (validator != null && !validator(value)))
        {
            Console.Write("Please enter a valid number: ");
            input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input) && defaultValue.HasValue)
            {
                return defaultValue.Value;
            }
        }
        
        return value;
    }

    /// <summary>
    /// Prompts the user for a string input with validation.
    /// </summary>
    /// <param name="promptText">The text to display as a prompt.</param>
    /// <param name="defaultValue">Optional default value to use if the user presses Enter.</param>
    /// <param name="validator">Optional validation function.</param>
    /// <returns>The validated string input.</returns>
    public string PromptForString(string promptText, string? defaultValue = null, Func<string, bool>? validator = null)
    {
        var defaultDisplay = !string.IsNullOrEmpty(defaultValue) ? $" [{defaultValue}]" : "";
        Console.Write($"{promptText}{defaultDisplay}: ");

        var input = Console.ReadLine()!.Trim();
        if (string.IsNullOrWhiteSpace(input) && !string.IsNullOrEmpty(defaultValue))
        {
            return defaultValue;
        }
        
        while (validator != null && !validator(input))
        {
            Console.Write("Please enter a valid value: ");
            input = Console.ReadLine()!.Trim();
            if (string.IsNullOrWhiteSpace(input) && !string.IsNullOrEmpty(defaultValue))
            {
                return defaultValue;
            }
        }
        return input;
    }

    /// <summary>
    /// Prompts the user for a yes/no response.
    /// </summary>
    /// <param name="promptText">The text to display as a prompt.</param>
    /// <param name="defaultValue">Optional default value to use if the user presses Enter.</param>
    /// <returns>True for yes, false for no.</returns>
    public bool PromptForYesNo(string promptText, bool? defaultValue = null)
    {
        var defaultDisplay = defaultValue.HasValue ? $" [{(defaultValue.Value ? "y" : "n")}]" : "";
        Console.Write($"{promptText}{defaultDisplay}: ");
        
        var input = Console.ReadLine()?.ToLower();
        if (string.IsNullOrWhiteSpace(input) && defaultValue.HasValue)
        {
            return defaultValue.Value;
        }
        
        return input is "y" or "yes";
    }

    /// <summary>
    /// Prompts the user to select an option from a list of choices.
    /// </summary>
    /// <param name="promptText">The text to display as a prompt.</param>
    /// <param name="choices">The list of choices to present to the user.</param>
    /// <param name="defaultValue">Optional default value to use if the user presses Enter.</param>
    /// <returns>The selected choice.</returns>
    public string PromptForChoice(string promptText, IEnumerable<string> choices, string? defaultValue = null)
    {
        var choicesList = choices.ToList();
        if (choicesList.Count == 0)
        {
            throw new ArgumentException("Choices cannot be empty", nameof(choices));
        }
        
        if (defaultValue != null && !choicesList.Contains(defaultValue, StringComparer.OrdinalIgnoreCase))
        {
            defaultValue = choicesList.First();
        }
        
        var defaultDisplay = defaultValue != null ? $" [{defaultValue}]" : "";
        Console.Write($"{promptText}{defaultDisplay}: ");

        var input = Console.ReadLine()!.Trim();
        if (string.IsNullOrWhiteSpace(input) && defaultValue != null)
        {
            return defaultValue;
        }
        
        while (!choicesList.Contains(input, StringComparer.OrdinalIgnoreCase))
        {
            Console.Write($"Please enter one of {string.Join(", ", choicesList)}: ");
            input = Console.ReadLine()!.Trim();
            if (string.IsNullOrWhiteSpace(input) && defaultValue != null)
            {
                return defaultValue;
            }
        }
        return input;
    }
}