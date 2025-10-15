namespace VisionaryCoder.Framework.Abstractions;

/// <summary>
/// Represents a strongly-typed integer identifier following Microsoft domain modeling patterns.
/// </summary>
/// <typeparam name="T">The type this identifier represents for type discrimination.</typeparam>
public abstract record IntId<T> : StronglyTypedId<int, IntId<T>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IntId{T}"/> class.
    /// </summary>
    /// <param name="value">The integer value.</param>
    /// <exception cref="ArgumentException">Thrown when the value is less than or equal to zero.</exception>
    protected IntId(int value) : base(value)
    {
        if (value <= 0)
            throw new ArgumentException("Integer identifier must be greater than zero.", nameof(value));
    }
}