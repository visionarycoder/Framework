public sealed class EfFilterExecutionStrategy(DbContext dbContext) : IFilterExecutionStrategy
{
    public IQueryable<T> Apply<T>(IQueryable<T> source, FilterNode? filter)
    {
        if (filter is null) return source;

        var parameter = Expression.Parameter(typeof(T), "x");
        var body = EfFilterExpressionBuilder.BuildExpression<T>(filter, parameter, dbContext);
        if (body is null) return source;

        var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
        return source.Where(lambda);
    }

    public IEnumerable<T> Apply<T>(IEnumerable<T> source, FilterNode? filter)
    {
        // reuse same expression, compile to func for in-memory:
        if (filter is null) return source;
        var parameter = Expression.Parameter(typeof(T), "x");
        var body = EfFilterExpressionBuilder.BuildExpression<T>(filter, parameter, dbContext);
        if (body is null) return source;

        var lambda = Expression.Lambda<Func<T, bool>>(body, parameter).Compile();
        return source.Where(lambda);
    }
}
