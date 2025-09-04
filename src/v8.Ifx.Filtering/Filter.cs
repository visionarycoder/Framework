namespace Wsdot.Idl.Ifx.Filtering.v3;

public class Filter
{
    public static Filter Empty => new();

    public CriterionCollection Criteria { get; init; } = [];
    public OrderByCollection OrderBy { get; init; } = [];
    public Paging Paging { get; set; } = new();
}

public class Filter<T> : Filter where T : new()
{

}