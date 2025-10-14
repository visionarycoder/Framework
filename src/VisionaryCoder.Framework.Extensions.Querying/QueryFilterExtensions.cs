namespace VisionaryCoder.Framework.Extensions.Querying;

public static class QueryFilterExtensions
{
    public static IQueryable<T> Apply<T>(this IQueryable<T> source, QueryFilter<T> filter) =>
        source.Where(filter.Predicate);

    public static IQueryable<T> ApplyAll<T>(this IQueryable<T> source, IEnumerable<QueryFilter<T>> filters)
    {
        var query = source;
        foreach (var f in filters) query = query.Where(f.Predicate);
        return query;
    }
}
