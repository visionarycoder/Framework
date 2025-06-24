using System.CommandLine;
using Microsoft.Extensions.Options;
using v9.Ifx.Services.OS.Windows.Cli.Menu;
using v9.Tool.Generator.Strings.Models;
using vc.Ifx.Services.Clipboard;
using vc.Ifx.Services.Configuration;
using vc.Ifx.Services.Generators;

namespace v9.Tool.Generator.Strings;

public class GeneratorClient(StringGeneratorService stringGeneratorService, IClipboardHelper clipboardHelper, IMenuService menuService, IConfigurationService configurationService, IOptions<LastExecution> lastExecutionOptions)
{

    private readonly LastExecution lastExecution = lastExecutionOptions.Value;

    public async Task RunAsync(string[] args)
    {
        var rootCommand = CreateRootCommand();
        await rootCommand.InvokeAsync(args);
    }

    public RootCommand CreateRootCommand()
    {
        // Create root command
        var rootCommand = new RootCommand("String generator application that creates strings based on specified parameters.");

        // Add options
        var lengthOption = new Option<int>(aliases: ["--length", "-l"], description: "Length of the string to generate. Must be a positive number.") { IsRequired = false };

        // Add validator for length option to ensure it's positive when provided
        lengthOption.AddValidator(result =>
        {
            if (result.GetValueOrDefault<int>() < 0)
            {
                result.ErrorMessage = "Length must be a positive number.";
            }
        });

        var typeOption = new Option<string>(aliases: ["--type", "-t"], description: "Type of characters to use (numeric or alpha).") { IsRequired = false };

        // Improve validator for type option
        typeOption.AddValidator(result =>
        {
            var value = result.GetValueOrDefault<string>()?.ToLower();
            if (!string.IsNullOrEmpty(value) && value != "numeric" && value != "alpha")
            {
                result.ErrorMessage = "Type must be either 'numeric' or 'alpha'.";
            }
        });

        var randomOption = new Option<bool>(aliases: ["--random", "-r"], description: "Generate a randomized string instead of a pattern.");

        // Add options to command
        rootCommand.AddOption(lengthOption);
        rootCommand.AddOption(typeOption);
        rootCommand.AddOption(randomOption);

        rootCommand.SetHandler((length, type, isRandom) =>
        {

            var argsProvided = length > 0 || !string.IsNullOrEmpty(type);

            // If parameters are missing, prompt the user with defaults from settings
            if (length <= 0)
            {
                length = menuService.PromptForInteger("Enter the length of the output", lastExecution.Length > 0 ? lastExecution.Length : null, value => value > 0);
            }

            if (string.IsNullOrEmpty(type))
            {
                // Validate stored type if it exists
                var defaultType = string.Empty;
                if (!string.IsNullOrEmpty(lastExecution.Type))
                {
                    var storedType = lastExecution.Type.ToLower();
                    defaultType = storedType is "numeric" or "alpha" ? storedType : "alpha";
                }
                type = menuService.PromptForChoice(
                    "Enter the type of output",
                    ["numeric", "alpha"],
                    defaultType);
            }

            // For randomOption, ask only if not provided via command line
            if (!argsProvided && !randomOption.IsRequired)
            {
                isRandom = menuService.PromptForYesNo(
                    "Do you want randomized output? (y/n)",
                    lastExecution.Random);
            }

            // Generate the string using our helper
            var generatedString = stringGeneratorService.Generate(length, type, isRandom);

            // Display the generated string
            Console.WriteLine("\nGenerated String:");
            Console.WriteLine(generatedString);

            // Try to copy to clipboard using our helper
            try
            {
                clipboardHelper.CopyToClipboard(generatedString);
                Console.WriteLine("String copied to clipboard!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not copy to clipboard: {ex.Message}");
            }

            // Update the settings with the current values
            lastExecution.Length = length;
            lastExecution.Type = type;
            lastExecution.Random = isRandom;

            // Save the updated settings using the configuration service
            configurationService.UpdateSection("LastExecution", lastExecution);
            configurationService.SaveChanges();
        }, lengthOption, typeOption, randomOption);

        return rootCommand;

    }

}