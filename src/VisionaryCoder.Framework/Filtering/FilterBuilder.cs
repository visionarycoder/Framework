using System.Linq.Expressions;

namespace VisionaryCoder.Framework.Filtering;

public sealed class FilterBuilder<T>
{
    private readonly List<FilterNode> roots = [];

    public FilterBuilder<T> Where(Expression<Func<T, bool>> predicate)
    {
        FilterNode node = ExpressionToFilterNode.Translate(predicate);
        roots.Add(node);
        return this;
    }

    public FilterNode Build()
    {
        return roots.Count switch
        {
            0 => new FilterGroup(FilterCombination.And, []),
            1 => roots[0],
            _ => new FilterGroup(FilterCombination.And, roots)
        };
    }
}
