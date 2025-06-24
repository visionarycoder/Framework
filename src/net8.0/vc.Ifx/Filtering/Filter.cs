using System.Diagnostics;

using vc.Ifx.Collections;

namespace vc.Ifx.Filtering;

/// <summary>
/// Manages a collection of filtering collection.
/// </summary>
public class FilterCriteria
{

    private readonly List<Criterion> collection = [];

    /// <summary>
    /// Gets the collection of collection.
    /// </summary>
    public IReadOnlyList<Criterion> Criteria => collection;

    public bool Add(Criterion criterion)
    {

        Trace.TraceInformation($"{nameof(Add)} called with Criterion={criterion}");
        if(string.IsNullOrWhiteSpace(criterion.PropertyName))
        {
            Trace.TraceWarning($"Invalid criterion with empty PropertyName");
            return false;
        }

        var matching = collection.FirstOrDefault(c => c.PropertyName == criterion.PropertyName);
        if(matching is null)
        {
            collection.Add(criterion);
            Trace.TraceInformation($"Added new criterion: {criterion.PropertyName}");
            return true;
        }

        Trace.TraceWarning($"{criterion.PropertyName} already exists.");
        return false;

    }

    /// <summary>
    /// Adds a criterion to the collection, replacing any existing criterion with the same property name.
    /// </summary>
    /// <param name="criterion">The criterion to add.</param>
    public bool AddOrUpdate(Criterion criterion)
    {

        if(string.IsNullOrWhiteSpace(criterion.PropertyName))
        {
            Trace.TraceWarning($"Invalid criterion with empty PropertyName");
            return false;
        }

        var matching = collection.FirstOrDefault(c => c.PropertyName == criterion.PropertyName);
        if(matching is null)
        {
            collection.Add(criterion);
            Trace.TraceInformation($"Adding new criterion: {criterion.PropertyName}");
            return collection.Contains(criterion);
        }
        
        var idx = collection.IndexOf(matching);
        collection.Remove(matching);
        collection.Insert(idx, criterion);
        Trace.TraceInformation($"Updated existing criterion: {criterion.PropertyName}");
        return collection.Contains(criterion);

    }

    /// <summary>
    /// Adds multiple collection to the collection.
    /// </summary>
    /// <param name="criteria">The collection to add.</param>
    public bool Add(params Criterion[] criteria)
    {
        if(criteria.Length == 0)
        {
            Trace.WriteLine("No criteria provided to add.");
            return false;
        }

        var allSucceeded = true;
        foreach(var criterion in criteria)
        {
            if(! Add(criterion))
            {
                allSucceeded = false;
            }
        }
        return allSucceeded;
    }

    public bool AddOrUpdate(ICollection<Criterion> criteria)
    {
        if(criteria.Count == 0)
        {
            Trace.WriteLine("No criteria provided to add.");
            return false;
        }

        var allSucceeded = true;
        foreach(var criterion in criteria)
        {
            if(! AddOrUpdate(criterion))
            {
                allSucceeded = false;
            }
        }
        return allSucceeded;
    }

    /// <summary>
    /// Clears all collection from the collection.
    /// </summary>
    public void Clear() => collection.Clear();

    /// <summary>
    /// Represents a filtering criterion.
    /// </summary>
    /// <param name="PropertyName">The name of the property to filter on.</param>
    /// <param name="PropertyValue">The value to compare against.</param>
    /// <param name="ComparisonOperator">The type of comparison to perform.</param>
    /// <param name="IgnoreCase">Whether to ignore case when comparing strings.</param>
    public record Criterion(string PropertyName, object? PropertyValue, ComparisonType ComparisonOperator = ComparisonType.Undefined, IgnoreCase IgnoreCase = IgnoreCase.Undefined)
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    /// <summary>
    /// Defines comparison types for filtering.
    /// </summary>
    public enum ComparisonType
    {
        Undefined = -1,
        Equals = 0,
        NotEquals,
        GreaterThan,
        LessThan,
        Contains
    }

    /// <summary>
    /// Defines case sensitivity options for string comparisons.
    /// </summary>
    public enum IgnoreCase
    {
        Undefined = -1,
        No = 0,
        Yes = 1
    }

}

/// <summary>
/// Manages a collection of ordering specifications.
/// </summary>
public class OrderByCollection
{

    private readonly List<OrderByProperty> collection = [];

    /// <summary>
    /// Gets the collection of ordering specifications.
    /// </summary>
    public IReadOnlyList<OrderByProperty> OrderBy => collection;

    /// <summary>
    /// Adds an ordering specification, replacing any existing one with the same property name.
    /// </summary>
    /// <param name="item">The ordering specification to add.</param>
    public void Add(OrderByProperty item)
    {
        if(string.IsNullOrWhiteSpace(item.PropertyName))
        {
            Trace.TraceWarning($"Invalid item with empty PropertyName");
            return;
        }

        var matching = collection.FirstOrDefault(c => c.PropertyName == item.PropertyName);
        if(matching is null)
        {
            collection.Add(item);
        }
        else
        {
            var idx = collection.IndexOf(matching);
            collection.Remove(matching);
            collection.Insert(idx, item);
        }
    }

    /// <summary>
    /// Adds multiple ordering specifications.
    /// </summary>
    /// <param name="items">The ordering specifications to add.</param>
    public void Add(params OrderByProperty[] items)
    {
        if(items.Length == 0)
            return;
        items.ForEach(Add);
    }

    /// <summary>
    /// Clears all ordering specifications.
    /// </summary>
    public void Clear() => collection.Clear();

    /// <summary>
    /// Represents an ordering specification.
    /// </summary>
    /// <param name="PropertyName">The name of the property to order by.</param>
    /// <param name="SortDirection">The direction to sort.</param>
    public record OrderByProperty(string PropertyName, SortDirection SortDirection = SortDirection.Ascending);

    /// <summary>
    /// Defines sort directions.
    /// </summary>
    public enum SortDirection
    {
        Ascending = 0,
        Descending
    }
}

/// <summary>
/// Handles pagination parameters for data queries.
/// </summary>
public class Pagination
{
    private int? skip;
    private int? take;

    /// <summary>
    /// Gets or sets the number of items to skip.
    /// </summary>
    public int? Skip
    {
        get => skip;
        set => skip = value < 0 ? null : value;
    }

    /// <summary>
    /// Gets or sets the number of items to take.
    /// </summary>
    public int? Take
    {
        get => take;
        set => take = value < 0 ? null : value;
    }

    /// <summary>
    /// Gets or sets the page number (1-based).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Updates Skip and Take based on Page and PageSize.
    /// </summary>
    public void UpdateSkipTakeFromPage()
    {
        Skip = ( Page - 1 ) * PageSize;
        Take = PageSize;
    }

    /// <summary>
    /// Updates Page based on Skip and Take.
    /// </summary>
    public void UpdatePageFromSkipTake()
    {
        if(Take is not > 0)
            return;
        Page = ( Skip ?? 0 ) / Take.Value + 1;
        PageSize = Take.Value;
    }

    /// <summary>
    /// Clears pagination settings.
    /// </summary>
    public void Clear()
    {
        Skip = null;
        Take = null;
        Page = 1;
        PageSize = 10;
    }
}

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



