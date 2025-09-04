using System.Collections;
using System.Globalization;

namespace Wsdot.Idl.Ifx.Filtering.v3;

public readonly record struct Criterion(string PropertyName, object? PropertyValue, string Operator = OperatorKeys.Equals, StringComparison StringComparison = StringComparison.CurrentCulture);

public readonly record struct ComparisonContext(StringComparison StringComparison, CultureInfo? Culture = null)
{

    public CompareOptions CompareOptions => StringComparison switch
    {
        StringComparison.CurrentCultureIgnoreCase or
        StringComparison.InvariantCultureIgnoreCase or
        StringComparison.OrdinalIgnoreCase => CompareOptions.IgnoreCase,
        _ => CompareOptions.None
    };

    public CultureInfo EffectiveCulture => StringComparison switch
    {
        StringComparison.InvariantCulture or StringComparison.InvariantCultureIgnoreCase => CultureInfo.InvariantCulture,
        _ => Culture ?? CultureInfo.CurrentCulture
    };

}

public static class ComparisonOperators
{

    public delegate bool OperatorFunc(object? left, object? right, in ComparisonContext ctx);

    private static readonly Dictionary<string, OperatorFunc> _ops = new(StringComparer.OrdinalIgnoreCase)
    {
        [OperatorKeys.Equals] = EqualsOp,
        [OperatorKeys.Eq] = EqualsOp,

        [OperatorKeys.NotEquals] = NotEqualsOp,
        [OperatorKeys.Neq] = NotEqualsOp,

        [OperatorKeys.GreaterThan] = GreaterThanOp,
        [OperatorKeys.Gt] = GreaterThanOp,

        [OperatorKeys.LessThan] = LessThanOp,
        [OperatorKeys.Lt] = LessThanOp,

        [OperatorKeys.Contains] = ContainsOp
    };

    public static bool TryGet(string key, out OperatorFunc func) => _ops.TryGetValue(key, out func);

    public static void Register(string key, OperatorFunc func) => _ops[key] = func;

    // Built-ins

    private static bool EqualsOp(object? a, object? b, in ComparisonContext ctx)
    {
        if (a is string sa && b is string sb)
            return string.Equals(sa, sb, ctx.StringComparison);

        if (a is IComparable comparable && b is not null)
            return comparable.CompareTo(b) == 0;

        return Equals(a, b);
    }

    private static bool NotEqualsOp(object? a, object? b, in ComparisonContext ctx) => !EqualsOp(a, b, ctx);

    private static bool GreaterThanOp(object? a, object? b, in ComparisonContext _) => CompareComparable(a, b) > 0;

    private static bool LessThanOp(object? a, object? b, in ComparisonContext _) => CompareComparable(a, b) < 0;

    private static bool ContainsOp(object? a, object? b, in ComparisonContext ctx)
    {

        if (a is string sa && b is string sb)
        {
            var compareInfo = ctx.EffectiveCulture.CompareInfo;
            return compareInfo.IndexOf(sa, sb, ctx.CompareOptions) >= 0;
        }

        if (a is IEnumerable enumerable)
        {
            return enumerable.Cast<object?>().Contains(b);
        }

        return false;

    }

    private static int CompareComparable(object? a, object? b)
    {

        switch (a)
        {
            case null when b is null: return 0;
            case null: return -1;
        }
        if (a is IComparable cmp)
            return cmp.CompareTo(b);

        if (b is null)
            return 1;
        throw new InvalidOperationException($"Type '{a.GetType()}' is not comparable.");

    }

}