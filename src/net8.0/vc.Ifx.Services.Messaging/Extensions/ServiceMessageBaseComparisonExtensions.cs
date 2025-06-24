using vc.Ifx.Services.Messaging.Models.Base;

namespace vc.Ifx.Services.Messaging.Extensions;

/// <summary>
/// Provides extension methods for comparing ServiceMessageBase instances.
/// </summary>
public static class ServiceMessageBaseComparisonExtensions
{

    /// <summary>
    /// Determines whether two ServiceMessageBase instances are equivalent.
    /// </summary>
    public static bool IsEquivalentTo(this ServiceMessageBase? left, ServiceMessageBase? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.MessageId.Equals(right.MessageId)
            && left.CorrelationId.Equals(right.CorrelationId)
            && left.TimestampUtc.Equals(right.TimestampUtc)
            && left.MessageType == right.MessageType
            && left.SourceSystem == right.SourceSystem;

    }

    /// <summary>
    /// Gets a hash code for a ServiceMessageBase instance based on its comparison properties.
    /// </summary>
    public static int GetComparisonHashCode(this ServiceMessageBase obj)
    {
        return HashCode.Combine(obj.MessageId, obj.CorrelationId, obj.TimestampUtc, obj.MessageType, obj.SourceSystem);
    }

}