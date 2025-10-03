namespace vc.Ifx.Data;

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