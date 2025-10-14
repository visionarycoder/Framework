namespace VisionaryCoder.Framework.Abstractions;

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