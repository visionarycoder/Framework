using System.Diagnostics;
using vc.Ifx.Collections;

namespace vc.Ifx.Filtering;

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