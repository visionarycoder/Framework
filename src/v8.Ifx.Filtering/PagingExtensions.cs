namespace Wsdot.Idl.Ifx.Filtering.v3;

public static class PagingExtensions
{
    public static bool HasDefinedSkip(this Paging source) => source.Skip > 0;
    public static bool HasDefinedTake(this Paging source) => source.Take is not null && source.Take > 0;
}