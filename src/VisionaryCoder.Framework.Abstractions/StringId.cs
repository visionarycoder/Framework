namespace VisionaryCoder.Framework.Abstractions;

/// <summary>
/// Represents a strongly-typed string identifier following Microsoft domain modeling patterns.
/// </summary>
/// <typeparam name="T">The type this identifier represents for type discrimination.</typeparam>
public abstract record StringId<T> : StronglyTypedId<string, StringId<T>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringId{T}"/> class.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <exception cref="ArgumentException">Thrown when the value is null, empty, or whitespace.</exception>
    protected StringId(string value) : base(value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
    }
}