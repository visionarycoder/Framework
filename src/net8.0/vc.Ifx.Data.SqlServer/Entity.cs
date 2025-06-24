namespace vc.Ifx.Data;

public abstract partial class Entity
{
    public long RowVersion { get; set; }
}

public readonly struct GuidId(GuidId value) : IComparable<GuidId>, IEquatable<GuidId>
{

    public Guid Value { get; } = value;

    public static GuidId New() => new GuidId(Guid.NewGuid());

    public bool Equals(GuidId other) => this.Value.Equals(other.Value);
    public int CompareTo(GuidId other) => Value.CompareTo(other.Value);

    public override bool Equals(object obj)
    {
        if(ReferenceEquals(null, obj))
            return false;
        return obj is GuidId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    public static bool operator ==(GuidId a, GuidId b) => a.CompareTo(b) == 0;
    public static bool operator !=(GuidId a, GuidId b) => !( a == b );

}

public readonly struct IntId(int value) : IComparable<IntId>, IEquatable<IntId>
{

    public int Value { get; } = value;

    public static IntId New() => new IntId(Guid.NewGuid());

    public bool Equals(IntId other) => this.Value.Equals(other.Value);
    public int CompareTo(IntId other) => Value.CompareTo(other.Value);

    public override bool Equals(object obj)
    {
        if(ReferenceEquals(null, obj))
            return false;
        return obj is IntId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    public static bool operator ==(IntId a, IntId b) => a.CompareTo(b) == 0;
    public static bool operator !=(IntId a, IntId b) => !( a == b );

}

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