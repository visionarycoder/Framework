namespace VisionaryCoder.Framework.Filtering;

/// <summary>
/// Represents a filter condition that operates on a collection property.
/// </summary>
/// <param name="Path">The path to the collection property (e.g., "Children" or "Orders.Items").</param>
/// <param name="Operator">The collection operator (Any, All, HasElements).</param>
/// <param name="Predicate">Optional nested filter to apply to collection elements. Required for Any/All, null for HasElements.</param>
public sealed record FilterCollectionCondition(string Path, FilterOperator Operator, FilterNode? Predicate) : FilterNode;
