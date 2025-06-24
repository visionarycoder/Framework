using System;
using System.Text;
// ReSharper disable ClassNeverInstantiated.Global

namespace vc.Ifx.Services.Generators;

/// <summary>
/// Provides methods for generating strings based on specified parameters.
/// </summary>
public class StringGeneratorService : ServiceBase, IStringGeneratorService
{

    private const string NUMERIC_CHARS = "0123456789";
    private const string ALPHA_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// Generates a string based on the specified parameters.
    /// </summary>
    /// <param name="length">The length of the string to generate.</param>
    /// <param name="type">The type of characters to use ("numeric" or "alpha").</param>
    /// <param name="isRandom">Whether to generate a random string or a repeating pattern.</param>
    /// <returns>The generated string.</returns>
    public string Generate(int length, string type, bool isRandom)
    {

        if (length <= 0)
        {
            return string.Empty;
        }

        var charSet = type.ToLowerInvariant() == "numeric" ? NUMERIC_CHARS : ALPHA_CHARS;
        var result = new StringBuilder(length);

        if (isRandom)
        {
            var random = Random.Shared; // Use shared random instance for better performance
            for (var i = 0; i < length; i++)
            {
                result.Append(charSet[random.Next(charSet.Length)]);
            }
        }
        else
        {
            // For non-random, create a repeating pattern
            for (var i = 0; i < length; i++)
            {
                result.Append(charSet[i % charSet.Length]);
            }
        }
        return result.ToString();

    }

}