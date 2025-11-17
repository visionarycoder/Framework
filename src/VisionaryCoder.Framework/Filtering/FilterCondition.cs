namespace VisionaryCoder.Framework.Filtering;

public sealed record FilterCondition(string Path, FilterOperator Operator, string? Value) : FilterNode;
