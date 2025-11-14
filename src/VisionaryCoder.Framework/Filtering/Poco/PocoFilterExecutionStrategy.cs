public sealed class PocoFilterExecutionStrategy : IFilterExecutionStrategy
{
    public IQueryable<T> Apply<T>(IQueryable<T> source, FilterNode? filter)
    {
        if (filter is null) return source;

        var parameter = Expression.Parameter(typeof(T), "x");
        var body = PocoFilterExpressionBuilder.BuildExpression<T>(filter, parameter);
        if (body is null) return source;

        var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
        return source.Where(lambda);
    }

    public IEnumerable<T> Apply<T>(IEnumerable<T> source, FilterNode? filter)
    {
        if (filter is null) return source;
        var parameter = Expression.Parameter(typeof(T), "x");
        var body = PocoFilterExpressionBuilder.BuildExpression<T>(filter, parameter);
        if (body is null) return source;

        var lambda = Expression.Lambda<Func<T, bool>>(body, parameter).Compile();
        return source.Where(lambda);
    }
}
