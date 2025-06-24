using System.Diagnostics;
using vc.v2.Ifx.Core;

namespace Ifx.Filtering;

public class Filter 
{
    public static Filter Empty => new();

    private readonly List<Criterion> criterionCollection = new();
    public IReadOnlyList<Criterion> Criteria => criterionCollection;

    private readonly List<OrderByProperty> orderByCollection = new();
    public IReadOnlyList<OrderByProperty> OrderBy => orderByCollection;

    private int? skip;
    public int? Skip
    {
        get => skip;
        set => skip = value < 0 ? null : value;
    }

    private int? take;
    public int? Take
    {
        get => take;
        set => take = value < 0 ? null : value;
    }

    public void AddCriterion(Criterion item)
    {
        if (string.IsNullOrWhiteSpace(item.PropertyName))
        {
            Debug.WriteLine($"The input orderByClause has an invalid PropertyName: '{item.PropertyName}'");
            return;
        }
        var matching = criterionCollection.FirstOrDefault(c => c.PropertyName == item.PropertyName);
        if (matching is null)
        {
            criterionCollection.Add(item);
        }
        else
        {
            var idx = criterionCollection.IndexOf(matching);
            criterionCollection.Remove(matching);
            criterionCollection.Insert(idx,item);
        }
    }

    public void AddCriteria(params Criterion[] items)
    {
        if(items.Length == 0)
            return;
        items.Where(c => true).ToList().ForEach(AddCriterion);
    }

    public void AddOrderByClause(OrderByProperty item)
    {
        if (string.IsNullOrWhiteSpace(item.PropertyName))
        {
            Debug.WriteLine($"The input orderByClause has an invalid PropertyName: '{item.PropertyName}'");
            return;
        }
        var matching = orderByCollection.FirstOrDefault(c => c.PropertyName == item.PropertyName);
        if (matching is null)
        {
            orderByCollection.Add(item);
        }
        else
        {
            var idx = orderByCollection.IndexOf(matching);
            orderByCollection.Remove(matching);
            orderByCollection.Insert(idx, item);
        }
    }

    public void AddOrderBy(params OrderByProperty[] items)
    {
        if (items == null || items.Length == 0)
            return;
        items.Where(c => true).ToList().ForEach(AddOrderByClause);
    }

    public record Criterion(string PropertyName, object? PropertyValue, ComparisonType ComparisonOperator = ComparisonType.Undefined, IgnoreCase IgnoreCase = IgnoreCase.Undefined);

    public record OrderByProperty(string PropertyName, SortDirection SortDirection = SortDirection.Undefined);

    public enum ComparisonType
    {
        Undefined = -1,
        Equals = 0,
        NotEquals,
        GreaterThan,
        LessThan,
        Contains
    }

    public enum IgnoreCase
    {
        Undefined = -1,
        No = 0,
        Yes = 1
    }

    public enum SortDirection
    {
        Undefined = -1,
        Ascending = 0,
        Descending
    }

}

/// <summary>
///     Represents a generic filter for querying data.
///     Includes criteria for filtering, pagination, and ordering.
/// </summary>
public class Filter<T> : Filter
    where T : class
{


}