namespace Wsdot.Idl.Ifx.Filtering.v3;

public record Paging(int Skip = 0, int? Take = null)
{
    private static readonly Paging empty = new();
    public static Paging Empty { get; } = empty;
}