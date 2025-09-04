using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Wsdot.Idl.Ifx.Filtering.v3;

public static class QueryableExtensions
{

    // Get MethodInfo references for ToString, ToLower, and Contains
    private static readonly MethodInfo toStringMethod = typeof(object).GetMethod(FilterConstants.METHOD_NAME_TO_STRING, Type.EmptyTypes)!;
    private static readonly MethodInfo toLowerMethod = typeof(string).GetMethod(FilterConstants.METHOD_NAME_TO_LOWER, Type.EmptyTypes)!;
    private static readonly MethodInfo containsMethod = typeof(string).GetMethod(FilterConstants.METHOD_NAME_CONTAINS, [typeof(string)])!;

    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Filter<T> filter)
        where T : IComparable<T>, new()
    {
        query = query.ApplyCriteria(filter);
        query = query.ApplyOrdering(filter);
        query = query.ApplyPaging(filter);
        return query;
    }

    public static IQueryable<T> ApplyCriteria<T>(this IQueryable<T> query, Filter<T> filter)
        where T : IComparable<T>, new()
    {
        foreach (var criterion in filter.Criteria)
        {
            InvalidCriterionException.ThrowIfNullOrWhitespace(criterion.PropertyName);
            var predicate = BuildPredicate<T>(criterion);
            query = query.Where(predicate);
        }

        return query;
    }

    public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, Filter<T> filter)
        where T : IComparable<T>, new()
    {
        if (filter.OrderBy.Count == 0)
            return query;

        query = query.ApplyOrderBy(filter.OrderBy[0]);
        foreach (var orderByProperty in filter.OrderBy[1..])
        {
            query = query.ApplyThenBy(orderByProperty);
        }

        return query;
    }

    public static IQueryable<T> ApplyOrderBy<T>(this IQueryable<T> query, OrderByProperty orderByProperty)
        where T : new()
    {
        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(parameter, orderByProperty.PropertyName);
        var lambda = Expression.Lambda(property, parameter);
        var methodName = orderByProperty.SortDirection == ListSortDirection.Ascending
            ? FilterConstants.ORDER_BY
            : FilterConstants.ORDER_BY_DESCENDING;
        var resultExpression = Expression.Call(typeof(Queryable), methodName, [typeof(T), property.Type], query.Expression, Expression.Quote(lambda));
        return query.Provider.CreateQuery<T>(resultExpression);
    }

    public static IQueryable<T> ApplyThenBy<T>(this IQueryable<T> query, OrderByProperty orderByProperty)
        where T : new()
    {
        var parameter = Expression.Parameter(typeof(T), FilterConstants.EXPRESSION_PARAMETER);
        var property = Expression.Property(parameter, orderByProperty.PropertyName);
        var lambda = Expression.Lambda(property, parameter);
        var methodName = orderByProperty.SortDirection == ListSortDirection.Ascending
            ? FilterConstants.THEN_BY
            : FilterConstants.THEN_BY_DESCENDING;
        var resultExpression = Expression.Call(typeof(Queryable), methodName, [typeof(T), property.Type], query.Expression, Expression.Quote(lambda));
        return query.Provider.CreateQuery<T>(resultExpression);
    }

    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, Filter filter)
        where T : new()
    {
        var skipIsValid = false;
        if (filter.Paging.Skip is > 0)
        {
            // Add skip value
            query = query.Skip(filter.Paging.Skip);
            skipIsValid = true;
        }

        // If take is null or less than or equal to 0, return the query without 'Take()'
        if (filter.Paging.Take is null or <= 0)
            return query;

        // If take is valid, but skip is not, set skip to 0
        if (!skipIsValid)
            query = query.Skip(0);
        return query.Take(filter.Paging.Take.Value);
    }

    private static Expression<Func<T, bool>> BuildPredicate<T>(Criterion criterion)
        where T : new()
    {
        var parameter = Expression.Parameter(typeof(T), FilterConstants.EXPRESSION_PARAMETER);
        var property = Expression.Property(parameter, criterion.PropertyName);
        var constant = Expression.Constant(criterion.PropertyValue);

        Expression body = criterion.Operator switch
        {
            OperatorKeys.Equals => Expression.Equal(property, constant),
            OperatorKeys.NotEquals => Expression.NotEqual(property, constant),
            OperatorKeys.GreaterThan => Expression.GreaterThan(property, constant),
            OperatorKeys.LessThan => Expression.LessThan(property, constant),
            OperatorKeys.Contains when criterion.StringComparison is StringComparison.CurrentCultureIgnoreCase or StringComparison.InvariantCultureIgnoreCase or StringComparison.OrdinalIgnoreCase => BuildCaseInsensitiveContains(property, constant),
            OperatorKeys.Contains => Expression.Call(property, FilterConstants.METHOD_NAME_CONTAINS, null, constant),
            var _ => throw new NotSupportedException($"Operator {criterion.Operator} is not supported.")
        };

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    /// <summary>
    ///     Builds a case-insensitive "Contains" expression.
    /// </summary>
    /// <param name="property">The property expression.</param>
    /// <param name="constant">The constant expression.</param>
    /// <returns>The case-insensitive "Contains" expression.</returns>
    private static MethodCallExpression BuildCaseInsensitiveContains(Expression property, Expression constant)
    {
        // Convert property to string and to lowercase
        var propertyToString = Expression.Call(property, toStringMethod);
        var propertyToLower = Expression.Call(propertyToString, toLowerMethod);

        // Convert constant to string and to lowercase
        var constantToString = Expression.Call(constant, toStringMethod);
        var constantToLower = Expression.Call(constantToString, toLowerMethod);

        // Build the "Contains" call
        return Expression.Call(propertyToLower, containsMethod, constantToLower);
    }

}