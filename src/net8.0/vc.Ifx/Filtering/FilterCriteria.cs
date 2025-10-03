using System.Diagnostics;

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