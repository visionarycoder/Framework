using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace VisionaryCoder.Framework.Querying;

/// <summary>
/// Extension methods for composing and creating <see cref="QueryFilter{T}"/> instances.
/// </summary>
/// <remarks>
/// These helpers let you build small, reusable predicates and then compose them into more
/// complex filters that work with LINQ providers (including EF Core).
/// <para>
/// Typical flow: create simple filters (Contains/StartsWith/EndsWith), combine with And/Or/Not,
/// then apply to an <see cref="IQueryable{T}"/> using <see cref="Apply"/> or <see cref="ApplyAll"/>.
/// </para>
/// <example>
/// <code>
/// // Given an entity
/// public sealed record User(int Id, string Name, string Email);
/// 
/// // Build filters
/// var hasAnn   = QueryFilterExtensions.ContainsIgnoreCase&lt;User&gt;(u =&gt; u.Name, "ann");
/// var endsWith = QueryFilterExtensions.EndsWithIgnoreCase&lt;User&gt;(u =&gt; u.Email, ".org");
/// 
/// // Compose
/// var filter = hasAnn.And(endsWith);
/// 
/// // Apply
/// IQueryable&lt;User&gt; users = db.Users; // any IQueryable provider
/// var result = users.Apply(filter).ToList();
/// </code>
/// </example>
/// </remarks>
public static class QueryFilterExtensions
{
    /// <summary>
    /// Combines two filters with a logical AND.
    /// </summary>
    /// <example>
    /// <code>
    /// var byName = QueryFilterExtensions.Contains&lt;User&gt;(u =&gt; u.Name, "ann");
    /// var byMail = QueryFilterExtensions.EndsWith&lt;User&gt;(u =&gt; u.Email, ".org");
    /// var both   = byName.And(byMail);
    /// </code>
    /// </example>
    public static QueryFilter<T> And<T>(this QueryFilter<T> left, QueryFilter<T> right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        var parameter = left.Predicate.Parameters[0];
        var rightBody = right.Predicate.Body.ReplaceParameter(right.Predicate.Parameters[0], parameter);
        var body = Expression.AndAlso(left.Predicate.Body, rightBody);
        return new QueryFilter<T>(Expression.Lambda<Func<T, bool>>(body, parameter));
    }

    /// <summary>
    /// Combines two filters with a logical OR.
    /// </summary>
    /// <example>
    /// <code>
    /// var byGmail = QueryFilterExtensions.EndsWithIgnoreCase&lt;User&gt;(u =&gt; u.Email, "@gmail.com");
    /// var byYahoo = QueryFilterExtensions.EndsWithIgnoreCase&lt;User&gt;(u =&gt; u.Email, "@yahoo.com");
    /// var either  = byGmail.Or(byYahoo);
    /// </code>
    /// </example>
    public static QueryFilter<T> Or<T>(this QueryFilter<T> left, QueryFilter<T> right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        var parameter = left.Predicate.Parameters[0];
        var rightBody = right.Predicate.Body.ReplaceParameter(right.Predicate.Parameters[0], parameter);
        var body = Expression.OrElse(left.Predicate.Body, rightBody);
        return new QueryFilter<T>(Expression.Lambda<Func<T, bool>>(body, parameter));
    }

    /// <summary>
    /// Negates a filter with a logical NOT (!).
    /// </summary>
    /// <example>
    /// <code>
    /// var freeEmail = QueryFilterExtensions.ContainsIgnoreCase&lt;User&gt;(u =&gt; u.Email, "@gmail.com");
    /// var corporate = freeEmail.Not(); // everything that does NOT match
    /// </code>
    /// </example>
    public static QueryFilter<T> Not<T>(this QueryFilter<T> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        var parameter = filter.Predicate.Parameters[0];
        var body = Expression.Not(filter.Predicate.Body);
        return new QueryFilter<T>(Expression.Lambda<Func<T, bool>>(body, parameter));
    }

    /// <summary>
    /// Creates a filter where the selected string property contains the specified value.
    /// When the value is null or empty, returns a filter that always evaluates to true (no-op).
    /// </summary>
    /// <example>
    /// <code>
    /// var hasAnn = QueryFilterExtensions.Contains&lt;User&gt;(u =&gt; u.Name, "ann");
    /// var users  = query.Apply(hasAnn);
    /// </code>
    /// </example>
    public static QueryFilter<T> Contains<T>(Expression<Func<T, string>> selector, string? value)
    {
        ArgumentNullException.ThrowIfNull(selector);
        if (string.IsNullOrWhiteSpace(value))
        {
            return True<T>();
        }

        var param = selector.Parameters[0];
        var constant = Expression.Constant(value, typeof(string));
        var body = Expression.Call(selector.Body, nameof(string.Contains), Type.EmptyTypes, constant);
        return new QueryFilter<T>(Expression.Lambda<Func<T, bool>>(body, param));
    }

    /// <summary>
    /// Creates a filter where the selected string property starts with the specified value.
    /// When the value is null or empty, returns a filter that always evaluates to true (no-op).
    /// </summary>
    /// <example>
    /// <code>
    /// var startsWithA = QueryFilterExtensions.StartsWith&lt;User&gt;(u =&gt; u.Name, "A");
    /// var result = query.Apply(startsWithA).ToList();
    /// </code>
    /// </example>
    public static QueryFilter<T> StartsWith<T>(Expression<Func<T, string>> selector, string? value)
    {
        ArgumentNullException.ThrowIfNull(selector);
        if (string.IsNullOrWhiteSpace(value))
        {
            return True<T>();
        }

        var param = selector.Parameters[0];
        var constant = Expression.Constant(value, typeof(string));
        var body = Expression.Call(selector.Body, nameof(string.StartsWith), Type.EmptyTypes, constant);
        return new QueryFilter<T>(Expression.Lambda<Func<T, bool>>(body, param));
    }

    /// <summary>
    /// Creates a filter where the selected string property ends with the specified value.
    /// When the value is null or empty, returns a filter that always evaluates to true (no-op).
    /// </summary>
    /// <example>
    /// <code>
    /// var endsWithOrg = QueryFilterExtensions.EndsWith&lt;User&gt;(u =&gt; u.Email, ".org");
    /// var result = query.Apply(endsWithOrg);
    /// </code>
    /// </example>
    public static QueryFilter<T> EndsWith<T>(Expression<Func<T, string>> selector, string? value)
    {
        ArgumentNullException.ThrowIfNull(selector);
        if (string.IsNullOrWhiteSpace(value))
        {
            return True<T>();
        }

        var param = selector.Parameters[0];
        var constant = Expression.Constant(value, typeof(string));
        var body = Expression.Call(selector.Body, nameof(string.EndsWith), Type.EmptyTypes, constant);
        return new QueryFilter<T>(Expression.Lambda<Func<T, bool>>(body, param));
    }

    /// <summary>
    /// Joins multiple filters using AND semantics by default. If <paramref name="useAnd"/> is false, uses OR semantics.
    /// Empty sequences yield an always-true filter.
    /// </summary>
    /// <example>
    /// <code>
    /// var filters = new []
    /// {
    ///     QueryFilterExtensions.ContainsIgnoreCase&lt;User&gt;(u =&gt; u.Name, "ann"),
    ///     QueryFilterExtensions.EndsWithIgnoreCase&lt;User&gt;(u =&gt; u.Email, ".org")
    /// };
    /// var combined = filters.Join(useAnd: true);
    /// </code>
    /// </example>
    public static QueryFilter<T> Join<T>(this IEnumerable<QueryFilter<T>> filters, bool useAnd = true)
    {
        ArgumentNullException.ThrowIfNull(filters);
        using var e = filters.GetEnumerator();
        if (!e.MoveNext())
        {
            return True<T>();
        }

        var current = e.Current ?? True<T>();
        while (e.MoveNext())
        {
            var next = e.Current ?? True<T>();
            current = useAnd ? current.And(next) : current.Or(next);
        }
        return current;
    }

    /// <summary>
    /// Joins multiple filters using AND semantics by default. If <paramref name="useAnd"/> is false, uses OR semantics.
    /// </summary>
    public static QueryFilter<T> Join<T>(bool useAnd, params QueryFilter<T>[] filters)
        => (filters ?? Array.Empty<QueryFilter<T>>()).Join(useAnd);

    private static QueryFilter<T> True<T>()
    {
        var p = Expression.Parameter(typeof(T), "x");
        return new QueryFilter<T>(Expression.Lambda<Func<T, bool>>(Expression.Constant(true), p));
    }

    private static Expression ReplaceParameter(this Expression expression, ParameterExpression source, ParameterExpression target)
        => new ParameterReplacer(source, target).Visit(expression)!;

    private sealed class ParameterReplacer(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
            => node == source ? target : base.VisitParameter(node);
    }

    // Case-insensitive string helpers
    /// <summary>
    /// Creates a filter where the selected string property contains the specified value (case-insensitive).
    /// Returns an always-true filter if the value is null or empty.
    /// </summary>
    /// <example>
    /// <code>
    /// var hasAnn = QueryFilterExtensions.ContainsIgnoreCase&lt;User&gt;(u =&gt; u.Name, "Ann");
    /// </code>
    /// </example>
    public static QueryFilter<T> ContainsIgnoreCase<T>(Expression<Func<T, string>> selector, string? value)
    {
        ArgumentNullException.ThrowIfNull(selector);
        if (string.IsNullOrWhiteSpace(value)) return True<T>();

        var param = selector.Parameters[0];
        // x => x.Prop != null && x.Prop.ToLowerInvariant().Contains(value.ToLowerInvariant())
        var toLowerInvariant = typeof(string).GetMethod(nameof(string.ToLowerInvariant), Type.EmptyTypes)!;
        var contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        var notNull = Expression.NotEqual(selector.Body, Expression.Constant(null, typeof(string)));
        var left = Expression.Call(selector.Body, toLowerInvariant);
        var right = Expression.Constant(value.ToLowerInvariant());
        var containsCall = Expression.Call(left, contains, right);
        var body = Expression.AndAlso(notNull, containsCall);
        return new QueryFilter<T>(Expression.Lambda<Func<T, bool>>(body, param));
    }

    /// <summary>
    /// Creates a filter where the selected string property starts with the specified value (case-insensitive).
    /// Returns an always-true filter if the value is null or empty.
    /// </summary>
    /// <example>
    /// <code>
    /// var startsWith = QueryFilterExtensions.StartsWithIgnoreCase&lt;User&gt;(u =&gt; u.Name, "an");
    /// </code>
    /// </example>
    public static QueryFilter<T> StartsWithIgnoreCase<T>(Expression<Func<T, string>> selector, string? value)
    {
        ArgumentNullException.ThrowIfNull(selector);
        if (string.IsNullOrWhiteSpace(value)) return True<T>();

        var param = selector.Parameters[0];
        var toLowerInvariant = typeof(string).GetMethod(nameof(string.ToLowerInvariant), Type.EmptyTypes)!;
        var startsWith = typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) })!;

        var notNull = Expression.NotEqual(selector.Body, Expression.Constant(null, typeof(string)));
        var left = Expression.Call(selector.Body, toLowerInvariant);
        var right = Expression.Constant(value.ToLowerInvariant());
        var call = Expression.Call(left, startsWith, right);
        var body = Expression.AndAlso(notNull, call);
        return new QueryFilter<T>(Expression.Lambda<Func<T, bool>>(body, param));
    }

    /// <summary>
    /// Creates a filter where the selected string property ends with the specified value (case-insensitive).
    /// Returns an always-true filter if the value is null or empty.
    /// </summary>
    /// <example>
    /// <code>
    /// var endsWith = QueryFilterExtensions.EndsWithIgnoreCase&lt;User&gt;(u =&gt; u.Email, ".org");
    /// </code>
    /// </example>
    public static QueryFilter<T> EndsWithIgnoreCase<T>(Expression<Func<T, string>> selector, string? value)
    {
        ArgumentNullException.ThrowIfNull(selector);
        if (string.IsNullOrWhiteSpace(value)) return True<T>();

        var param = selector.Parameters[0];
        var toLowerInvariant = typeof(string).GetMethod(nameof(string.ToLowerInvariant), Type.EmptyTypes)!;
        var endsWith = typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) })!;

        var notNull = Expression.NotEqual(selector.Body, Expression.Constant(null, typeof(string)));
        var left = Expression.Call(selector.Body, toLowerInvariant);
        var right = Expression.Constant(value.ToLowerInvariant());
        var call = Expression.Call(left, endsWith, right);
        var body = Expression.AndAlso(notNull, call);
        return new QueryFilter<T>(Expression.Lambda<Func<T, bool>>(body, param));
    }

    // IQueryable helpers
    /// <summary>
    /// Applies a single filter to an IQueryable.
    /// </summary>
    /// <example>
    /// <code>
    /// IQueryable&lt;User&gt; users = db.Users;
    /// var filter = QueryFilterExtensions.ContainsIgnoreCase&lt;User&gt;(u =&gt; u.Name, "ann");
    /// var filtered = users.Apply(filter);
    /// </code>
    /// </example>
    public static IQueryable<T> Apply<T>(this IQueryable<T> source, QueryFilter<T> filter)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(filter);
        return source.Where(filter.Predicate);
    }

    /// <summary>
    /// Applies multiple filters to an IQueryable in sequence using AND semantics.
    /// Null filters are ignored.
    /// </summary>
    /// <example>
    /// <code>
    /// var filters = new []
    /// {
    ///     QueryFilterExtensions.ContainsIgnoreCase&lt;User&gt;(u =&gt; u.Name, "ann"),
    ///     QueryFilterExtensions.EndsWithIgnoreCase&lt;User&gt;(u =&gt; u.Email, ".org")
    /// };
    /// var filtered = db.Users.ApplyAll(filters);
    /// </code>
    /// </example>
    public static IQueryable<T> ApplyAll<T>(this IQueryable<T> source, IEnumerable<QueryFilter<T>> filters)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(filters);
        var query = source;
        foreach (var f in filters)
        {
            if (f is null) continue;
            query = query.Where(f.Predicate);
        }
        return query;
    }
}
