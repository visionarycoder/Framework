using Microsoft.AspNetCore.Mvc.ModelBinding;
using VisionaryCoder.Framework.Primitives;

namespace VisionaryCoder.Framework.Primitives.AspNetCore;
public sealed class EntityIdModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext ctx)
        => ctx.Metadata.ModelType.IsGenericType
           && ctx.Metadata.ModelType.GetGenericTypeDefinition() == typeof(EntityId<,>)
            ? new EntityIdModelBinder() : null;
}
