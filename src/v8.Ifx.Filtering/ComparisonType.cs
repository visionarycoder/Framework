namespace Wsdot.Idl.Ifx.Filtering.v3;

// Replaces the old enum with string keys used by the operator registry.
public static class OperatorKeys
{
    public const string Equals = "equals";
    public const string NotEquals = "notequals";
    public const string GreaterThan = "greaterthan";
    public const string LessThan = "lessthan";
    public const string Contains = "contains";

    // Optional short aliases
    public const string Eq = "eq";
    public const string Neq = "neq";
    public const string Gt = "gt";
    public const string Lt = "lt";
}