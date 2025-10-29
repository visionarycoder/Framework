using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisionaryCoder.Framework.Primitives;

namespace VisionaryCoder.Framework.Primitives.Data.EFCore;
public static class EntityIdModelBuilderExtensions
{
    public static PropertyBuilder<EntityId<TEntity, TKey>> UseEntityId<TEntity, TKey>(
        this PropertyBuilder<EntityId<TEntity, TKey>> builder)
        where TEntity : class
        where TKey : notnull
    {
        var converter = new EntityIdValueConverter<TEntity, TKey>();
        var comparer = new ValueComparer<EntityId<TEntity, TKey>>(
            (a, b) => EqualityComparer<TKey>.Default.Equals(a.Value, b.Value),
            v => v.Value.GetHashCode(),
            v => new EntityId<TEntity, TKey>(v.Value));
        builder.HasConversion(converter);
        builder.Metadata.SetValueComparer(comparer);
        return builder;
    }
}
