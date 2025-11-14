using System.Linq;
using System.Linq.Expressions;

namespace VisionaryCoder.Framework.Filtering.Serialization;

public static class ExpressionToFilterNode
{

    public static FilterNode Translate<T>(Expression<Func<T, bool>> expression) => TranslateNode(expression.Body) ?? throw new NotSupportedException($"Expression '{expression}' is not supported.");
    public static FilterNode Translate(Expression expression) => TranslateNode(expression) ?? throw new NotSupportedException($"Expression '{expression}' is not supported.");

    static FilterNode? TranslateNode(Expression expression) =>
        expression switch
        {
            BinaryExpression binary => TranslateBinary(binary),
            MethodCallExpression call => TranslateMethodCall(call),
            UnaryExpression unary when unary.NodeType == ExpressionType.Not
                => TranslateNot(unary),
            _ => null
        };

    static FilterNode? TranslateBinary(BinaryExpression binary)
    {
        // Logical group: && / ||
        if (binary.NodeType is ExpressionType.AndAlso or ExpressionType.OrElse)
        {
            var combination = binary.NodeType == ExpressionType.AndAlso
                ? FilterCombination.And
                : FilterCombination.Or;

            var left = TranslateNode(binary.Left);
            var right = TranslateNode(binary.Right);

            var children = new List<FilterNode>();
            if (left is not null) children.AddRange(FlattenIfSameGroup(left, combination));
            if (right is not null) children.AddRange(FlattenIfSameGroup(right, combination));

            return new FilterGroup(combination, children);
        }
        // Comparison: ==, !=, <, <=, >, >=
        return TranslateComparison(binary);

    }

    static IEnumerable<FilterNode> FlattenIfSameGroup(FilterNode node, FilterCombination combination)
    {
        if (node is FilterGroup group && group.Combination == combination)
        {
            foreach (var child in group.Children)
            {
                yield return child;
            }
            yield break;
        }
        yield return node;
    }

    static FilterNode? TranslateComparison(BinaryExpression binary)
    {

        var (memberExpr, constantExpr, op) = NormalizeBinary(binary);
        if (memberExpr is null || constantExpr is null || op is null)
        {
            return null;
        }

        var path = GetMemberPath(memberExpr);
        if (path is null)
        {
            return null;
        }

        var value = EvaluateToString(constantExpr);
        return new FilterCondition(path, op.Value, value);

    }

    /// <summary>
    /// Normalizes a binary expression so that the member is on the left
    /// and the constant/value is on the right. Adjusts the operator if needed.
    /// </summary>
    static (MemberExpression? member, Expression? constant, FilterOperator?) NormalizeBinary(BinaryExpression binary)
    {
        var leftMember = GetMember(binary.Left);
        var rightMember = GetMember(binary.Right);

        var leftIsConstLike = IsConstantLike(binary.Left);
        var rightIsConstLike = IsConstantLike(binary.Right);

        // member op constant
        if (leftMember is not null && rightIsConstLike)
        {
            var op = MapComparisonOperator(binary.NodeType, invert: false);
            return (leftMember, binary.Right, op);
        }

        // constant op member -> invert operator
        if (rightMember is not null && leftIsConstLike)
        {
            var op = MapComparisonOperator(binary.NodeType, invert: true);
            return (rightMember, binary.Left, op);
        }

        return (null, null, null);
    }

    static FilterOperator? MapComparisonOperator(ExpressionType nodeType, bool invert)
    {

        return (nodeType, invert) switch
        {
            (ExpressionType.Equal, _) => FilterOperator.Equals,
            (ExpressionType.NotEqual, _) => FilterOperator.NotEquals,

            (ExpressionType.GreaterThan, false) => FilterOperator.GreaterThan,
            (ExpressionType.GreaterThan, true) => FilterOperator.LessThan,

            (ExpressionType.GreaterThanOrEqual, false) => FilterOperator.GreaterOrEqual,
            (ExpressionType.GreaterThanOrEqual, true) => FilterOperator.LessOrEqual,

            (ExpressionType.LessThan, false) => FilterOperator.LessThan,
            (ExpressionType.LessThan, true) => FilterOperator.GreaterThan,

            (ExpressionType.LessThanOrEqual, false) => FilterOperator.LessOrEqual,
            (ExpressionType.LessThanOrEqual, true) => FilterOperator.GreaterOrEqual,

            _ => null
        };

    }

    static FilterNode? TranslateMethodCall(MethodCallExpression call)
    {

        // string.Contains / StartsWith / EndsWith
        if (call.Object is not null &&
            call.Object.Type == typeof(string) &&
            call.Arguments.Count == 1)
        {
            var targetMember = GetMember(call.Object);
            if (targetMember is null)
            {
                return null;
            }

            var path = GetMemberPath(targetMember);
            if (path is null)
            {
                return null;
            }

            var arg = call.Arguments[0];
            var value = EvaluateToString(arg);

            var op = call.Method.Name switch
            {
                nameof(string.Contains) => FilterOperator.Contains,
                nameof(string.StartsWith) => FilterOperator.StartsWith,
                nameof(string.EndsWith) => FilterOperator.EndsWith,
                _ => (FilterOperator?)null
            };

            return op is null
                ? null
                : new FilterCondition(path, op.Value, value);
        }

        // Collection methods: Any(), All(), Contains()
        if (call.Method.DeclaringType == typeof(Enumerable))
        {
            return TranslateEnumerableMethod(call);
        }

        // Collection instance methods: Contains() on List/ICollection
        if (call.Object is not null && 
            IsCollectionType(call.Object.Type) &&
            call.Method.Name == nameof(List<object>.Contains) &&
            call.Arguments.Count == 1)
        {
            var collectionMember = GetMember(call.Object);
            if (collectionMember is null)
            {
                return null;
            }

            var path = GetMemberPath(collectionMember);
            if (path is null)
            {
                return null;
            }

            var value = EvaluateToString(call.Arguments[0]);
            return new FilterCondition(path, FilterOperator.Contains, value);
        }

        // Custom methods can be added here by checking call.Method.DeclaringType and Method.Name
        // Example for custom method support:
        // if (call.Method.DeclaringType == typeof(MyCustomClass) && call.Method.Name == "MyMethod")
        // {
        //     // Extract parameters and create appropriate FilterNode
        //     return new FilterCondition(...);
        // }
        return null;

    }

    static FilterNode? TranslateNot(UnaryExpression unary)
    {
        // Only handle simple negation of a comparison or method call for now
        // e.g. !c.IsActive or !c.Name.Contains("x")
        if (unary.Operand is BinaryExpression binary)
        {
            // Flip operator if possible
            var (memberExpr, constantExpr, op) = NormalizeBinary(binary);
            if (memberExpr is null || constantExpr is null || op is null)
            {
                return null;
            }

            var negated = NegateOperator(op.Value);
            var path = GetMemberPath(memberExpr);
            var value = EvaluateToString(constantExpr);

            return new FilterCondition(path!, negated, value);
        }

        if (unary.Operand is MethodCallExpression call)
        {
            // e.g. !c.Name.Contains("x") => NotContains (or NotEquals on Contains semantics)
            // For now, treat as NotEquals with Contains semantics if you like,
            // or just not support and return null.
            return null;
        }

        return null;
    }

    static FilterOperator NegateOperator(FilterOperator op) =>
        op switch
        {
            FilterOperator.Equals => FilterOperator.NotEquals,
            FilterOperator.NotEquals => FilterOperator.Equals,
            FilterOperator.GreaterThan => FilterOperator.LessOrEqual,
            FilterOperator.GreaterOrEqual => FilterOperator.LessThan,
            FilterOperator.LessThan => FilterOperator.GreaterOrEqual,
            FilterOperator.LessOrEqual => FilterOperator.GreaterThan,
            _ => throw new NotSupportedException($"Cannot negate operator '{op}'.")
        };

    static MemberExpression? GetMember(Expression expression) =>
        expression switch
        {
            MemberExpression m => m,
            UnaryExpression u when u.NodeType == ExpressionType.Convert && u.Operand is MemberExpression inner
                => inner,
            _ => null
        };

    static bool IsConstantLike(Expression expression) =>
        expression.NodeType is ExpressionType.Constant
        || expression is MemberExpression m && m.Expression is ConstantExpression
        || expression is UnaryExpression u && IsConstantLike(u.Operand);

    static string? GetMemberPath(MemberExpression member)
    {
        var parts = new Stack<string>();
        Expression? current = member;

        while (current is MemberExpression m)
        {
            parts.Push(m.Member.Name);
            current = m.Expression;
        }

        // Stop at the root parameter (e.g. x)
        return string.Join('.', parts);
    }

    /// <summary>
    /// Translates LINQ Enumerable method calls (Any, All, Contains) to FilterNode structures.
    /// </summary>
    /// <param name="call">The method call expression representing a LINQ Enumerable method.</param>
    /// <returns>A FilterNode representing the collection operation, or null if translation is not supported.</returns>
    static FilterNode? TranslateEnumerableMethod(MethodCallExpression call)
    {
        // First argument should be the collection (source)
        if (call.Arguments.Count == 0)
        {
            return null;
        }

        var collectionExpr = call.Arguments[0];
        var collectionMember = GetMember(collectionExpr);
        if (collectionMember is null)
        {
            return null;
        }

        var path = GetMemberPath(collectionMember);
        if (path is null)
        {
            return null;
        }

        switch (call.Method.Name)
        {
            case nameof(Enumerable.Any):
                // Any() without predicate - just check if collection has elements
                if (call.Arguments.Count == 1)
                {
                    return new FilterCollectionCondition(path, FilterOperator.HasElements, null);
                }
                // Any(predicate) - check if any element matches the predicate
                else if (call.Arguments.Count == 2 && call.Arguments[1] is UnaryExpression { Operand: LambdaExpression lambdaPredicate })
                {
                    var predicateFilter = TranslateNode(lambdaPredicate.Body);
                    if (predicateFilter is null)
                    {
                        return null;
                    }
                    return new FilterCollectionCondition(path, FilterOperator.Any, predicateFilter);
                }
                break;

            case nameof(Enumerable.All):
                // All(predicate) - check if all elements match the predicate
                if (call.Arguments.Count == 2 && call.Arguments[1] is UnaryExpression { Operand: LambdaExpression lambdaPredicate })
                {
                    var predicateFilter = TranslateNode(lambdaPredicate.Body);
                    if (predicateFilter is null)
                    {
                        return null;
                    }
                    return new FilterCollectionCondition(path, FilterOperator.All, predicateFilter);
                }
                break;

            case nameof(Enumerable.Contains):
                // Contains(value) - check if collection contains a specific value
                if (call.Arguments.Count == 2)
                {
                    var value = EvaluateToString(call.Arguments[1]);
                    return new FilterCondition(path, FilterOperator.Contains, value);
                }
                break;
        }

        return null;
    }

    /// <summary>
    /// Checks if a type is a collection type (array or implements IEnumerable&lt;T&gt;, ICollection&lt;T&gt;, or IList&lt;T&gt;).
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is a collection type (excluding string); otherwise, false.</returns>
    static bool IsCollectionType(Type type)
    {
        if (type == typeof(string))
        {
            return false;
        }

        return type.IsArray ||
               type.GetInterfaces().Any(i =>
                   i.IsGenericType &&
                   (i.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                    i.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                    i.GetGenericTypeDefinition() == typeof(IList<>)));
    }

    static string? EvaluateToString(Expression expression)
    {
        // Normalize to underlying expression
        Expression expr = expression;

        // Strip conversions
        while (expr is UnaryExpression u && expr.NodeType == ExpressionType.Convert)
        {
            expr = u.Operand;
        }

        if (expr is ConstantExpression constant)
        {
            return constant.Value?.ToString();
        }

        // Captured local / closure / more complex constant
        var lambda = Expression.Lambda(expr);
        var value = lambda.Compile().DynamicInvoke();
        return value?.ToString();
    }
}
