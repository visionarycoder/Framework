namespace Wsdot.Idl.Ifx.Filtering.v3;

public static class PagedExtensions
{
    public static int PageCount(this Paged source) => source.Take is > 0
        ? (int)Math.Ceiling((double)source.Skip / source.Take.Value)
        : 0;
    public static bool HasPreviousPage(this Paged source) => source.Skip > 0;
    public static bool HasNextPage(this Paged source, int totalCount) => (source.Skip + source.Take) < totalCount;
    public static int PageSize(this Paged source) => source.Take ?? 0;
    public static int PageIndex(this Paged source) => source.Skip / (source.Take ?? 1);
}