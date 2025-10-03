namespace vc.Ifx.Filtering;

/// <summary>
/// A composite filter for LINQ queries that combines criteria, ordering, and pagination.
/// </summary>
public class Filter
{
    /// <summary>
    /// Gets an empty filter.
    /// </summary>
    public static Filter Empty => new();

    /// <summary>
    /// Gets the criteria collection.
    /// </summary>
    public FilterCriteria FilterCriteria { get; } = new();

    /// <summary>
    /// Gets the ordering collection.
    /// </summary>
    public OrderByCollection OrderBy { get; } = new();

    /// <summary>
    /// Gets the pagination parameters.
    /// </summary>
    public Pagination Pagination { get; } = new();

    /// <summary>
    /// Clears all filter components.
    /// </summary>
    public void Clear()
    {
        FilterCriteria.Clear();
        OrderBy.Clear();
        Pagination.Clear();
    }
}



