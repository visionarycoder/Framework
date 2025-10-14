using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VisionaryCoder.Framework.Extensions.Primitives.AspNetCore;

public sealed class EntityIdModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext ctx)
        => ctx.Metadata.ModelType.IsGenericType
           && ctx.Metadata.ModelType.GetGenericTypeDefinition() == typeof(EntityId<,>)
            ? new EntityIdModelBinder() : null;
}
