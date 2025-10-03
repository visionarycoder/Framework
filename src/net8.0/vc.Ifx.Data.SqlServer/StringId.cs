namespace vc.Ifx.Data;

public readonly struct StringId(string value) : IComparable<StringId>, IEquatable<StringId>
{

    public int Value { get; } = value;

    public static StringId New() => new StringId(Guid.NewGuid());

    public bool Equals(StringId other) => this.Value.Equals(other.Value);
    public int CompareTo(StringId other) => Value.CompareTo(other.Value);

    public override bool Equals(object obj)
    {
        if(ReferenceEquals(null, obj))
            return false;
        return obj is StringId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    public static bool operator ==(StringId a, StringId b) => a.CompareTo(b) == 0;
    public static bool operator !=(StringId a, StringId b) => !( a == b );

}