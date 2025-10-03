namespace vc.Ifx.Data;

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