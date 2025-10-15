// VisionaryCoder.Framework.Extensions.Querying

using System.Linq.Expressions;

namespace VisionaryCoder.Framework.Extensions.Querying;

public sealed class QueryFilter<T>(Expression<Func<T, bool>> predicate)
{
    public Expression<Func<T, bool>> Predicate { get; } = predicate ?? throw new ArgumentNullException(nameof(predicate));
}
