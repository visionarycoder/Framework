using System.Linq.Expressions;
using System.Text.Json;

namespace VisionaryCoder.Framework.Querying.Serialization;

public abstract record FilterNode;

public sealed record PropertyFilter(
    string Operator,   // "Contains", "StartsWith", "EndsWith"
    string Property,   // e.g. "Name"
    string? Value,
    bool IgnoreCase = false
) : FilterNode;

public sealed record CompositeFilter(
    string Operator,   // "And", "Or", "Not"
    List<FilterNode> Children
) : FilterNode;

public static class QueryFilterSerializer
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static string Serialize(FilterNode node)
        => JsonSerializer.Serialize(node, options);

    public static FilterNode? Deserialize(string json)
        => JsonSerializer.Deserialize<FilterNode>(json, options);
}

public static class QueryFilterRehydrator
{
    public static QueryFilter<T> ToQueryFilter<T>(this FilterNode node)
    {
        return node switch
        {
            PropertyFilter pf => BuildPropertyFilter<T>(pf),
            CompositeFilter cf => BuildCompositeFilter<T>(cf),
            _ => throw new NotSupportedException($"Unknown filter node: {node?.GetType().Name}")
        };
    }

    private static QueryFilter<T> BuildPropertyFilter<T>(PropertyFilter pf)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var prop = Expression.PropertyOrField(param, pf.Property);
        var constant = Expression.Constant(pf.Value ?? string.Empty);

        Expression body = pf.Operator switch
        {
            "Contains" => CallStringMethod(prop, "Contains", constant, pf.IgnoreCase),
            "StartsWith" => CallStringMethod(prop, "StartsWith", constant, pf.IgnoreCase),
            "EndsWith" => CallStringMethod(prop, "EndsWith", constant, pf.IgnoreCase),
            _ => throw new NotSupportedException($"Unsupported operator {pf.Operator}")
        };

        return new QueryFilter<T>(Expression.Lambda<Func<T, bool>>(body, param));
    }

    private static QueryFilter<T> BuildCompositeFilter<T>(CompositeFilter cf)
    {
        if (cf.Operator == "Not" && cf.Children.Count == 1)
            return cf.Children[0].ToQueryFilter<T>().Not();

        if (cf.Operator == "And")
            return cf.Children.Select(c => c.ToQueryFilter<T>()).Join(useAnd: true);

        if (cf.Operator == "Or")
            return cf.Children.Select(c => c.ToQueryFilter<T>()).Join(useAnd: false);

        throw new NotSupportedException($"Unsupported composite operator {cf.Operator}");
    }

    private static Expression CallStringMethod(Expression prop, string method, ConstantExpression constant, bool ignoreCase)
    {
        if (ignoreCase)
        {
            var toLower = typeof(string).GetMethod(nameof(string.ToLowerInvariant), Type.EmptyTypes)!;
            var loweredProp = Expression.Call(prop, toLower);
            var loweredConst = Expression.Constant(((string)constant.Value!).ToLowerInvariant());
            return Expression.Call(loweredProp, typeof(string).GetMethod(method, new[] { typeof(string) })!, loweredConst);
        }
        return Expression.Call(prop, typeof(string).GetMethod(method, new[] { typeof(string) })!, constant);
    }
}

public static class QueryFilterValidator
{
    public static void ValidateOrThrow(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON cannot be null or whitespace.", nameof(json));

        try
        {
            using var document = JsonDocument.Parse(json);
            // Basic validation - structure exists
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Invalid JSON payload: {ex.Message}", ex);
        }
    }
}