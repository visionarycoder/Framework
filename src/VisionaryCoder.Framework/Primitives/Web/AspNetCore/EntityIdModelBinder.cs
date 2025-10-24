using Microsoft.AspNetCore.Mvc.ModelBinding;
using VisionaryCoder.Framework.Primitives;

namespace VisionaryCoder.Framework.Primitives.AspNetCore;
public sealed class EntityIdModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext ctx)
    {
        var text = ctx.ValueProvider.GetValue(ctx.ModelName).FirstValue;
        var t = ctx.ModelType; // EntityId<TEntity,TKey>
        if (string.IsNullOrWhiteSpace(text)) { ctx.Result = ModelBindingResult.Failed(); return Task.CompletedTask; }
        var parse = t.GetMethod("Parse", [typeof(string)])!;
        var value = parse.Invoke(null, [text]);
        ctx.Result = ModelBindingResult.Success(value);
        return Task.CompletedTask;
    }
}
