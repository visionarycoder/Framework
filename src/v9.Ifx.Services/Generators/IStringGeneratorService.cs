namespace vc.Ifx.Services.Generators;

public interface IStringGeneratorService : IService
{
    
    /// <summary>
    /// Generates a string based on the specified parameters.
    /// </summary>
    /// <param name="length">The length of the string to generate.</param>
    /// <param name="type">The type of characters to use ("numeric" or "alpha").</param>
    /// <param name="isRandom">Whether to generate a random string or a repeating pattern.</param>
    /// <returns>The generated string.</returns>
    string Generate(int length, string type, bool isRandom);

}