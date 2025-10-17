namespace VisionaryCoder.Framework.Abstractions;

/// <summary>
/// Provides a base class for strongly-typed identifier value objects following Microsoft domain modeling patterns.
/// Ensures type safety and prevents primitive obsession in domain models.
/// </summary>
/// <typeparam name="TValue">The underlying type of the identifier value.</typeparam>
/// <typeparam name="TId">The concrete identifier type for proper type discrimination.</typeparam>
public abstract record StronglyTypedId<TValue, TId> : IComparable<TId>
    where TValue : IComparable<TValue>, IEquatable<TValue>
    where TId : StronglyTypedId<TValue, TId>
{
    /// <summary>
    /// Gets the underlying value of this identifier.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StronglyTypedId{TValue, TId}"/> class.
    /// </summary>
    /// <param name="value">The underlying identifier value.</param>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    protected StronglyTypedId(TValue value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Compares this identifier to another identifier of the same type.
    /// </summary>
    /// <param name="other">The identifier to compare to.</param>
    /// <returns>A value indicating the relative order of the identifiers.</returns>
    public virtual int CompareTo(TId? other)
    {
        if (other is null) return 1;
        return Value.CompareTo(other.Value);
    }

    /// <summary>
    /// Returns the string representation of this identifier.
    /// </summary>
    /// <returns>The string representation of the underlying value.</returns>
    public override string ToString() => Value?.ToString() ?? string.Empty;

    /// <summary>
    /// Implicitly converts the identifier to its underlying value type.
    /// </summary>
    /// <param name="id">The identifier to convert.</param>
    /// <returns>The underlying value.</returns>
    public static implicit operator TValue(StronglyTypedId<TValue, TId> id) => id.Value;
}