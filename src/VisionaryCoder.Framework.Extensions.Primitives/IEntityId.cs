namespace VisionaryCoder.Framework.Extensions.Primitives;

public interface IEntityId
{
    Type ValueType { get; }
    object BoxedValue { get; }
}
