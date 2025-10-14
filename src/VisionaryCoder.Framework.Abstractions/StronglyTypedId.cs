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

/// <summary>
/// Represents a strongly-typed GUID identifier following Microsoft domain modeling patterns.
/// </summary>
/// <typeparam name="T">The type this identifier represents for type discrimination.</typeparam>
public abstract record GuidId<T> : StronglyTypedId<Guid, GuidId<T>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GuidId{T}"/> class.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <exception cref="ArgumentException">Thrown when the GUID is empty.</exception>
    protected GuidId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("GUID identifier cannot be empty.", nameof(value));
    }

    /// <summary>
    /// Creates a new GUID identifier with a generated value.
    /// </summary>
    /// <returns>A new identifier instance with a generated GUID.</returns>
    protected static TId New<TId>() where TId : GuidId<T>
    {
        var guid = Guid.NewGuid();
        return (TId)Activator.CreateInstance(typeof(TId), guid)!;
    }
}

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
