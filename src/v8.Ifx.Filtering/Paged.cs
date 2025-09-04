namespace Wsdot.Idl.Ifx.Filtering.v3;

public record Paged(int Count = 0) : Paging
{
    private static readonly Paged empty = new Paged();
    public new static Paged Empty { get; } = empty;
}