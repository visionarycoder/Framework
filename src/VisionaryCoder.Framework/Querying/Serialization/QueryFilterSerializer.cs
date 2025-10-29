using Json.Schema;
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
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static string Serialize(FilterNode node)
        => JsonSerializer.Serialize(node, Options);

    public static FilterNode? Deserialize(string json)
        => JsonSerializer.Deserialize<FilterNode>(json, Options);
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
    private static readonly JsonSchema Schema;

    static QueryFilterValidator()
    {
        string schemaJson = File.ReadAllText("queryfilter.schema.json");
        Schema = JsonSchema.FromText(schemaJson);
    }

    public static void ValidateOrThrow(string json)
    {
        var result = Schema.Evaluate(JsonDocument.Parse(json).RootElement, new EvaluationOptions
        {
            OutputFormat = OutputFormat.Detailed
        });

        if (!result.IsValid)
        {
            var errors = string.Join("; ", result.Details.Where(d => !d.IsValid).Select(d => d.InstanceLocation + ": " + d.Message));
            throw new InvalidOperationException($"Invalid QueryFilter payload: {errors}");
        }
    }
}