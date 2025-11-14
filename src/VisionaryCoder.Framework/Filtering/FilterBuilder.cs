using System.Linq.Expressions;

public sealed class FilterBuilder<T>
{
    readonly List<FilterNode> roots = new();

    public FilterBuilder<T> Where(Expression<Func<T, bool>> predicate)
    {
        var node = ExpressionToFilterNode.Translate(predicate);
        roots.Add(node);
        return this;
    }

    public FilterNode Build()
    {
        return roots.Count switch
        {
            0 => new FilterGroup(FilterCombination.And, Array.Empty<FilterNode>()),
            1 => roots[0],
            _ => new FilterGroup(FilterCombination.And, roots)
        };
    }
}
