using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VisionaryCoder.Framework.Primitives;

namespace VisionaryCoder.Framework.Primitives.Data.EFCore;
public sealed class EntityIdValueConverter<TEntity, TKey>() : ValueConverter<EntityId<TEntity, TKey>, TKey>(id => id.Value, v => new EntityId<TEntity, TKey>(v)) where TEntity : class where TKey : notnull;
