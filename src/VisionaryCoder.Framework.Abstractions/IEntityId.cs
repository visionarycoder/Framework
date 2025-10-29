namespace VisionaryCoder.Framework.Abstractions;

public interface IEntityId
{
    Type ValueType { get; }
    object BoxedValue { get; }
}
